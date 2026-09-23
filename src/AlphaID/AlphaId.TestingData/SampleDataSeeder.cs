using AlphaId.EntityFramework;
using AlphaId.EntityFramework.Admin;
using AlphaId.EntityFramework.IdSubjects;
using AlphaIdPlatform.Admin;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetTopologySuite.Geometries;
using Organizational;

namespace AlphaId.TestingData;

/// <summary>
/// 把 <see cref="SampleData"/> 写入数据库。
/// </summary>
/// <remarks>
/// <para>
/// 写入走 EF 实体而不是 SQL 脚本，因此与数据库提供程序无关（SQL Server / SQLite / 其他均可），
/// 同时也能顺带校验实体与表的映射关系是否仍然成立。
/// </para>
/// <para>
/// 三个重载分别对应三个 <see cref="DbContext"/>，可以各自独立调用；集成测试也可以只传要用的那几个对象，
/// 例如 <c>SampleDataSeeder.SeedAsync(db, [SampleData.LiuBei])</c>。
/// </para>
/// </remarks>
public static class SampleDataSeeder
{
    /// <summary>
    /// 写入样例自然人（含密码历史、外部登录、银行账户）。
    /// </summary>
    /// <param name="db">身份数据库上下文。</param>
    /// <param name="people">要写入的样例自然人，为 <see langword="null"/> 时写入 <see cref="SampleData.People"/> 全部。</param>
    /// <param name="cancellationToken">取消标记。</param>
    public static async Task SeedAsync(
        AlphaIdIdentityDbContext db,
        IEnumerable<SamplePerson>? people = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);

        foreach (SamplePerson sample in people ?? SampleData.People)
        {
            NaturalPerson person = CreatePerson(sample);
            EntityEntry<NaturalPerson> entry = db.Entry(person);
            // 这几个属性只有受保护的 setter，对象初始化器写不进去，只能经 EF 的属性入口赋值。
            // 必须在 Add 之前完成，否则改的是已被跟踪实体的主键。
            entry.Property(p => p.WhenCreated).CurrentValue = sample.WhenCreated;
            entry.Property(p => p.WhenChanged).CurrentValue = sample.WhenChanged;
            entry.Property(p => p.PasswordLastSet).CurrentValue = sample.PasswordLastSet;
            entry.Property(p => p.Locale).CurrentValue = sample.Locale;
            entry.Property(p => p.TimeZone).CurrentValue = sample.TimeZone;

            db.Users.Add(person);

            if (sample.PasswordHash is not null)
            {
                // 样例数据里每个有密码的人，其密码历史恰好只有一条与当前密码相同的记录。
                person.UsedPasswords.Add(new UsedPassword { PasswordHash = sample.PasswordHash });
            }

            foreach (SampleBankAccount account in sample.BankAccounts)
            {
                person.BankAccounts.Add(new NaturalPersonBankAccount
                {
                    AccountNumber = account.AccountNumber,
                    AccountName = account.AccountName,
                    BankName = account.BankName,
                });
            }

            foreach (SampleExternalLogin login in sample.ExternalLogins)
            {
                db.Set<IdentityUserLogin<string>>().Add(new IdentityUserLogin<string>
                {
                    UserId = sample.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    ProviderDisplayName = login.ProviderDisplayName,
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 写入样例组织（含成员与曾用名）。
    /// </summary>
    /// <param name="db">平台数据库上下文。</param>
    /// <param name="organizations">要写入的样例组织，为 <see langword="null"/> 时写入 <see cref="SampleData.Organizations"/> 全部。</param>
    /// <param name="cancellationToken">取消标记。</param>
    public static async Task SeedAsync(
        AlphaIdDbContext db,
        IEnumerable<SampleOrganization>? organizations = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);

        foreach (SampleOrganization sample in organizations ?? SampleData.Organizations)
        {
            Organization organization = new(sample.Name)
            {
                Domicile = sample.Domicile,
                Contact = sample.Contact,
                Representative = sample.Representative,
                EstablishedAt = sample.EstablishedAt,
                Location = CreateLocation(sample.Location),
                WhenChanged = sample.WhenChanged,
            };

            EntityEntry<Organization> entry = db.Entry(organization);
            // 同 CreatePerson：主键与创建时间只有受保护的 setter，需在 Add 之前赋值。
            entry.Property(o => o.Id).CurrentValue = sample.Id;
            entry.Property(o => o.WhenCreated).CurrentValue = sample.WhenCreated;

            db.Set<Organization>().Add(organization);

            foreach (SampleOrganizationMember member in sample.Members)
            {
                organization.Members.Add(new OrganizationMember(member.PersonId, member.Visibility)
                {
                    Department = member.Department,
                    Title = member.Title,
                    Remark = member.Remark,
                    IsOwner = member.IsOwner,
                });
            }

            foreach (SampleOrganizationUsedName usedName in sample.UsedNames)
            {
                organization.UsedNames.Add(new OrganizationUsedName
                {
                    Name = usedName.Name,
                    DeprecateTime = usedName.DeprecateTime,
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 写入样例角色分配。
    /// </summary>
    /// <param name="db">管理后台数据库上下文。</param>
    /// <param name="userInRoles">要写入的样例角色分配，为 <see langword="null"/> 时写入 <see cref="SampleData.UserInRoles"/> 全部。</param>
    /// <param name="cancellationToken">取消标记。</param>
    public static async Task SeedAsync(
        OperationalDbContext db,
        IEnumerable<SampleUserInRole>? userInRoles = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);

        foreach (SampleUserInRole sample in userInRoles ?? SampleData.UserInRoles)
        {
            db.UserInRoles.Add(new UserInRole { UserId = sample.UserId, RoleName = sample.RoleName });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static NaturalPerson CreatePerson(SamplePerson sample)
    {
        return new NaturalPerson(sample.UserName)
        {
            Id = sample.Id,
            UserName = sample.UserName,
            // 规范化字段由 UserManager 负责维护，这里按 ASP.NET Core Identity 默认的
            // UpperInvariantLookupNormalizer 规则派生，避免在各个样例里重复书写。
            NormalizedUserName = sample.UserName.ToUpperInvariant(),
            Email = sample.Email,
            NormalizedEmail = sample.Email?.ToUpperInvariant(),
            EmailConfirmed = sample.EmailConfirmed,
            PhoneNumber = sample.PhoneNumber,
            PhoneNumberConfirmed = sample.PhoneNumberConfirmed,
            LockoutEnabled = true,
            PasswordHash = sample.PasswordHash,
            SecurityStamp = sample.SecurityStamp,
            ConcurrencyStamp = sample.ConcurrencyStamp,
            FamilyName = sample.FamilyName,
            GivenName = sample.GivenName,
            Name = sample.Name,
            NickName = sample.NickName,
            Bio = sample.Bio,
            PhoneticSurname = sample.PhoneticSurname,
            PhoneticGivenName = sample.PhoneticGivenName,
            SearchHint = sample.SearchHint,
            Gender = sample.Gender,
            DateOfBirth = sample.DateOfBirth,
            LockoutEnd = sample.LockoutEnd,
            Address = sample.Address,
            ProfilePicture = CreateProfilePicture(sample),
        };
    }

    private static IdSubjects.BinaryDataInfo? CreateProfilePicture(SamplePerson sample)
    {
        if (sample.AvatarFileName is null || sample.AvatarMimeType is null)
        {
            return null;
        }

        return new IdSubjects.BinaryDataInfo(sample.AvatarMimeType, SampleAvatars.Read(sample.AvatarFileName));
    }

    private static Geometry? CreateLocation(SampleLocation? location)
    {
        return location is null
            ? null
            : new Point(location.Value.Longitude, location.Value.Latitude) { SRID = SampleLocation.Srid };
    }
}
