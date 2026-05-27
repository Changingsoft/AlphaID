using AlphaId.EntityFramework.DirectoryAccountManagement;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AlphaId.EntityFramework.IdSubjects;
using IdSubjects.DirectoryLogon;

namespace DatabaseTool.Migrators;

internal class DirectoryLogonDbMigrator(DirectoryLogonDbContext db, AlphaIdIdentityDbContext userDb) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] files = Directory.GetFiles("./TestingData/DirectoryLogonData", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));

        var liubei = await userDb.Users.FirstOrDefaultAsync(p => p.UserName == "liubei") ?? throw new Exception("找不到测试用户 liubei，请确保AddTestingData开关打开，并且确认AlphaIdentityDbContext的迁移在此迁移之前完成。");

        //尝试检测在localhost:389是否存在LDAP服务，如果存在，则尝试添加测试数据。
        using var ldapConnection = new System.DirectoryServices.Protocols.LdapConnection("localhost:389");
        try
        {
            ldapConnection.Bind();

            var directoryService = new DirectoryService
            {
                Name = "本地LDAP服务",
                Type = LdapType.ADLDS,
                ServerAddress = "localhost:389",
                RootDn = "DC=changingsoft,DC=com",
                DefaultUserAccountContainer = "DC=changingsoft,DC=com",
                UpnSuffix = "changingsoft.com"
            };

            await db.DirectoryServices.AddAsync(directoryService);

            using var entry = new System.DirectoryServices.DirectoryEntry("LDAP://localhost:389/CN=liubei,DC=changingsoft,DC=com");

            var directoryAccount = new DirectoryAccount(directoryService, liubei.Id, entry.Guid.ToString());
            await db.LogonAccounts.AddAsync(directoryAccount);

            await db.SaveChangesAsync();

        }
        catch (System.DirectoryServices.Protocols.LdapException)
        {
            //无法连接到LDAP服务，跳过添加测试数据。
        }
    }
}