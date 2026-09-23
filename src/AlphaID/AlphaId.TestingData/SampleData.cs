using System.Globalization;
using IdSubjects;
using Organizational;

namespace AlphaId.TestingData;

/// <summary>
/// 描述一名样例自然人的原始数据（与 EF 实体解耦的纯数据）。
/// </summary>
/// <remarks>
/// 未列出的字段一律为 NULL / 默认值，包括
/// <c>MiddleName</c>、<c>WebSite</c>、<c>PersonWhenChanged</c>、
/// <c>AccessFailedCount</c>、<c>TwoFactorEnabled</c>。
/// <c>NormalizedUserName</c> / <c>NormalizedEmail</c> 由 <see cref="SampleDataSeeder"/> 按
/// ASP.NET Core Identity 的默认规范化规则（<c>ToUpperInvariant</c>）派生，<c>LockoutEnabled</c> 恒为 true。
/// </remarks>
public sealed record SamplePerson
{
    /// <summary>用户 Id。</summary>
    public required string Id { get; init; }

    /// <summary>登录名。</summary>
    public required string UserName { get; init; }

    /// <summary>电子邮件地址。</summary>
    public string? Email { get; init; }

    /// <summary>手机号码。</summary>
    public string? PhoneNumber { get; init; }

    /// <summary>电子邮件是否已确认。</summary>
    public bool EmailConfirmed { get; init; }

    /// <summary>手机号码是否已确认。</summary>
    public bool PhoneNumberConfirmed { get; init; }

    /// <summary>姓。</summary>
    public required string FamilyName { get; init; }

    /// <summary>名。</summary>
    public required string GivenName { get; init; }

    /// <summary>全名。</summary>
    public required string Name { get; init; }

    /// <summary>昵称。</summary>
    public string? NickName { get; init; }

    /// <summary>个人经历。</summary>
    public string? Bio { get; init; }

    /// <summary>姓氏拼音。</summary>
    public string? PhoneticSurname { get; init; }

    /// <summary>名字拼音。</summary>
    public string? PhoneticGivenName { get; init; }

    /// <summary>检索提示。</summary>
    public string? SearchHint { get; init; }

    /// <summary>性别。</summary>
    public Gender? Gender { get; init; }

    /// <summary>出生日期。</summary>
    public DateOnly? DateOfBirth { get; init; }

    /// <summary>账户锁定截止时间；为 <see langword="null"/> 表示账户未被锁定。</summary>
    public DateTimeOffset? LockoutEnd { get; init; }

    /// <summary>区域和语言选项。</summary>
    public string? Locale { get; init; }

    /// <summary>IANA 时区名称。</summary>
    public string? TimeZone { get; init; }

    /// <summary>配送地址。</summary>
    public AddressInfo? Address { get; init; }

    /// <summary>注册时间。</summary>
    public required DateTimeOffset WhenCreated { get; init; }

    /// <summary>最后修改时间。</summary>
    public required DateTimeOffset WhenChanged { get; init; }

    /// <summary>最后一次设置密码的时间。</summary>
    public DateTimeOffset? PasswordLastSet { get; init; }

    /// <summary>
    /// 当前密码的哈希值。
    /// 取值为 <see cref="SampleData.PasswordHashA"/> 等常量，均为明文
    /// <see cref="SampleData.Password"/> 的合法哈希；为 <see langword="null"/> 表示该用户没有密码。
    /// </summary>
    public string? PasswordHash { get; init; }

    /// <summary>安全戳。</summary>
    public required string SecurityStamp { get; init; }

    /// <summary>并发戳。</summary>
    public required string ConcurrencyStamp { get; init; }

    /// <summary>头像资源文件名（形如 <c>liubei.jpg</c>），为 <see langword="null"/> 表示没有头像。</summary>
    public string? AvatarFileName { get; init; }

    /// <summary>头像的 MIME 类型。</summary>
    public string? AvatarMimeType { get; init; }

    /// <summary>外部登录（联邦身份）记录。</summary>
    public IReadOnlyList<SampleExternalLogin> ExternalLogins { get; init; } = [];

    /// <summary>银行账户。</summary>
    public IReadOnlyList<SampleBankAccount> BankAccounts { get; init; } = [];
}

/// <summary>描述一条外部登录记录。</summary>
public sealed record SampleExternalLogin
{
    /// <summary>登录提供程序。</summary>
    public required string LoginProvider { get; init; }

    /// <summary>提供程序内的用户键。</summary>
    public required string ProviderKey { get; init; }

    /// <summary>提供程序的显示名称。</summary>
    public string? ProviderDisplayName { get; init; }
}

/// <summary>描述一个银行账户。</summary>
public sealed record SampleBankAccount
{
    /// <summary>账号。</summary>
    public required string AccountNumber { get; init; }

    /// <summary>户名。</summary>
    public string? AccountName { get; init; }

    /// <summary>开户行。</summary>
    public string? BankName { get; init; }
}

/// <summary>描述一个样例组织的原始数据。</summary>
public sealed record SampleOrganization
{
    /// <summary>组织 Id。</summary>
    public required string Id { get; init; }

    /// <summary>名称。</summary>
    public required string Name { get; init; }

    /// <summary>住所。</summary>
    public string? Domicile { get; init; }

    /// <summary>联系方式。</summary>
    public string? Contact { get; init; }

    /// <summary>代表人。</summary>
    public string? Representative { get; init; }

    /// <summary>注册时间。</summary>
    public DateOnly? EstablishedAt { get; init; }

    /// <summary>地理位置。</summary>
    public SampleLocation? Location { get; init; }

    /// <summary>创建记录的时间。</summary>
    public required DateTimeOffset WhenCreated { get; init; }

    /// <summary>记录修改的时间。</summary>
    public required DateTimeOffset WhenChanged { get; init; }

    /// <summary>组织成员。</summary>
    public IReadOnlyList<SampleOrganizationMember> Members { get; init; } = [];

    /// <summary>曾用名。</summary>
    public IReadOnlyList<SampleOrganizationUsedName> UsedNames { get; init; } = [];
}

/// <summary>描述一条组织成员关系。</summary>
public sealed record SampleOrganizationMember
{
    /// <summary>自然人 Id。</summary>
    public required string PersonId { get; init; }

    /// <summary>职务。</summary>
    public string? Title { get; init; }

    /// <summary>部门。</summary>
    public string? Department { get; init; }

    /// <summary>备注。</summary>
    public string? Remark { get; init; }

    /// <summary>是否为组织的创建者。</summary>
    public bool IsOwner { get; init; }

    /// <summary>成员关系的可见性。</summary>
    public MembershipVisibility Visibility { get; init; } = MembershipVisibility.Private;
}

/// <summary>描述一个组织的曾用名。</summary>
public sealed record SampleOrganizationUsedName
{
    /// <summary>曾用名称。</summary>
    public required string Name { get; init; }

    /// <summary>弃用日期。</summary>
    public required DateOnly DeprecateTime { get; init; }
}

/// <summary>描述管理后台的一条用户角色分配。</summary>
public sealed record SampleUserInRole
{
    /// <summary>用户 Id。</summary>
    public required string UserId { get; init; }

    /// <summary>角色名称。</summary>
    public required string RoleName { get; init; }
}

/// <summary>WGS 84 坐标。</summary>
/// <param name="Longitude">经度。</param>
/// <param name="Latitude">纬度。</param>
/// <remarks>
/// 数值必须原样照抄历史数据，不能凑成整数或较短的小数：SQL Server 的 <c>geography</c>
/// 以二进制保存坐标，改动任何一位小数都会让序列化结果不同。
/// </remarks>
public readonly record struct SampleLocation(double Longitude, double Latitude)
{
    /// <summary>空间参考系标识。样例数据一律使用 WGS 84。</summary>
    public const int Srid = 4326;
}

/// <summary>
/// AlphaID 的样例数据。既是种子数据的唯一定义处，也是集成测试引用样例实体时的入口。
/// </summary>
/// <remarks>
/// <para>
/// 这些数据原先保存在 <c>DatabaseTool/TestingData</c> 下的 3 个 T-SQL 脚本里，
/// 由 <c>DatabaseTool</c> 以「读文件 + <c>ExecuteSqlRaw</c>」的方式灌入数据库。
/// 那套做法有三个问题：脚本依赖运行时当前目录、只有 SQL Server 能执行、
/// 二进制字段（头像）必须以十六进制文本内嵌（884 KB 文本对应 428 KB 二进制）。
/// </para>
/// <para>
/// 现在数据是 C# 常量，写入由 <see cref="SampleDataSeeder"/> 经 EF 实体完成，
/// 因此与数据库提供程序无关，也能被集成测试按用例裁剪（例如只灌 <see cref="LiuBei"/>）。
/// </para>
/// </remarks>
public static partial class SampleData
{
    /// <summary>全部样例账号共用的密码明文。</summary>
    public const string Password = "Pass123$";

    /// <summary>
    /// <see cref="Password"/> 的哈希值之一。样例数据中这三个哈希互不相同（盐与迭代次数不同），
    /// 但都能通过 <c>PasswordHasher</c> 校验为 <see cref="Password"/>。
    /// 保留原值是为了让种子数据与历史数据库逐字节一致。
    /// </summary>
    public const string PasswordHashA = "AQAAAAIAAYagAAAAELuwE7qoCVmVWyQtQCRwMKDmw+ZGO5azUW0sCI1KL9wtOUhys+loBDH05TNfaKpOMQ==";

    /// <summary><see cref="Password"/> 的哈希值之一。见 <see cref="PasswordHashA"/>。</summary>
    public const string PasswordHashB = "AQAAAAIAAYagAAAAEBo6J+rG5eTfCOYCw9mFEloJbvd3Ws42N3jhmGXX5JA80mNnXD33ViZRP62gakQDSA==";

    /// <summary><see cref="Password"/> 的哈希值之一。见 <see cref="PasswordHashA"/>。</summary>
    public const string PasswordHashC = "AQAAAAIAAYagAAAAEBaf3e7AFeBh5Ye7MVqe8waDodfU4tuJYdRikeZfbDgzUqOfpVEXtO3XIOSGnG/rzQ==";

    /// <summary>
    /// 把 <c>"O"</c> 格式的时间字符串解析为 <see cref="DateTimeOffset"/>（无损往返）。
    /// </summary>
    private static DateTimeOffset Time(string value)
    {
        return DateTimeOffset.ParseExact(value, "O", CultureInfo.InvariantCulture);
    }
}
