using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.DirectoryServices;

namespace IDSubjects.DirectoryLogon;

/// <summary>
/// Logon Account Manager.
/// </summary>
public class LogonAccountManager
{
    private readonly IDirectoryServiceStore directoryServiceStore;
    private readonly ILogonAccountStore logonAccountStore;
    private readonly ILogger<LogonAccountManager>? logger;
    private readonly NaturalPersonManager naturalPersonManager;
    private readonly LogonAccountManagerOptions options;
    /// <summary>
    /// Init.
    /// </summary>
    /// <param name="directoryServiceStore"></param>
    /// <param name="logonAccountStore"></param>
    /// <param name="naturalPersonManager"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public LogonAccountManager(IDirectoryServiceStore directoryServiceStore,
                               ILogonAccountStore logonAccountStore,
                               NaturalPersonManager naturalPersonManager,
                               IOptions<LogonAccountManagerOptions> options,
                               ILogger<LogonAccountManager>? logger = null)
    {
        this.directoryServiceStore = directoryServiceStore;
        this.logonAccountStore = logonAccountStore;
        this.naturalPersonManager = naturalPersonManager;
        this.options = options.Value;
        this.logger = logger;
    }

    /// <summary>
    /// Create account.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public async Task<LogonAccount> CreateAsync(NaturalPerson person, CreateAccountRequest request)
    {
        var directoryService = await this.directoryServiceStore.FindByIdAsync(request.ServiceId) ?? throw new InvalidOperationException("找不到指定的DirectoryService。");

        //todo 查找重名。//主要是sAMAccountName和userProncipalName不能重复
        try
        {
            using var userOu = directoryService.GetUserOuEntry();
            using var userEntry = userOu.Children.Add($"CN={request.AccountName}", "user");
            userEntry.Properties["displayName"].Value = request.DisplayName;
            userEntry.Properties["sn"].Value = request.Surname;
            userEntry.Properties["givenName"].Value = request.GivenName;
            userEntry.Properties["mobile"].Value = request.E164Mobile;
            if (request.Email != null)
                userEntry.Properties["mail"].Value = request.Email;
            userEntry.Properties["sAMAccountName"].Value = request.SamAccountName;
            userEntry.Properties["userPrincipalName"].Value = $"{request.UpnLeftPart}@{directoryService.UpnSuffix}";
            if (request.PinyinSurname != null)
                userEntry.Properties["msDS-PhoneticLastName"].Value = request.PinyinSurname;
            if (request.PinyinGivenName != null)
                userEntry.Properties["msDS-PhoneticFirstName"].Value = request.PinyinGivenName;
            var pinyin = $"{request.PinyinSurname} {request.PinyinGivenName}".Trim();
            if (!string.IsNullOrEmpty(pinyin))
                userEntry.Properties["msDS-PhoneticDisplayName"].Value = pinyin;

            userEntry.CommitChanges();
            //Set Init Password
            //const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
            //const long ADS_OPTION_PASSWORD_METHOD = 7;
            //const int ADS_PASSWORD_ENCODE_CLEAR = 1;
            //userEntry.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_PORTNUMBER, 389 });
            //userEntry.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR });

            userEntry.Invoke("SetPassword", new object[] { request.InitPassword });
            userEntry.CommitChanges();

            //set User Account Control Flag
            //userEntry.Properties["msDS-UserAccountDisabled"].Clear();  // normal account

            int val = (int)userEntry.Properties["userAccountControl"].Value!;
            userEntry.Properties["userAccountControl"].Value = val & ~0x0002;
            userEntry.Properties["pwdLastSet"][0] = 0;
            userEntry.CommitChanges();

            LogonAccount logonAccount = new()
            {
                ServiceId = directoryService.Id,
                LogonId = Convert.ToBase64String(userEntry.Guid.ToByteArray()),
                PersonId = person.Id,
            };
            await this.logonAccountStore.CreateAsync(logonAccount);

            //Create external login
            if (directoryService.ExternalLoginProvider != null)
            {
                var providerKey = this.options.GenerateExternalLoginId(directoryService.SAMDomainPart, userEntry);
                var identityResult = await this.naturalPersonManager.AddLoginAsync(person, new Microsoft.AspNetCore.Identity.UserLoginInfo(directoryService.ExternalLoginProvider, providerKey, directoryService.ExternalLoginProviderName));
                if (!identityResult.Succeeded)
                {
                    this.logger?.LogError("未能创建外部登录，错误消息：{errors}", identityResult.Errors.Select(p => p.Description));
                    throw new InvalidOperationException("未能创建外部登录。");
                }
            }

            return logonAccount;
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "向目录服务创建用户账户时出错");
            throw;
        }
        finally
        {

        }
    }

    /// <summary>
    /// Search from directory service.
    /// </summary>
    /// <param name="directoryService"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public IEnumerable<DirectorySearchItem> Search(DirectoryService directoryService, string filter)
    {
        using var searchRoot = directoryService.GetRootEntry();
        using DirectorySearcher searcher = new(searchRoot);
        searcher.Filter = filter;
        SearchResultCollection results = searcher.FindAll();
        HashSet<DirectorySearchItem> directorySearchItems = new();
        foreach (SearchResult searchResult in results)
        {
            using DirectoryEntry entry = searchResult.GetDirectoryEntry();
            directorySearchItems.Add(new(entry.Properties["name"].Value!.ToString()!,
                                         entry.Properties["sAMAccountName"].Value?.ToString(),
                                         entry.Properties["userPrincipalName"].Value!.ToString()!,
                                         entry.Guid,
                                         entry.Properties["distinguishedName"].Value!.ToString()!,
                                         entry.Properties["displayName"].Value?.ToString(),
                                         entry.Properties["mobile"].Value?.ToString(),
                                         entry.Properties["company"].Value?.ToString(),
                                         entry.Properties["department"].Value?.ToString(),
                                         entry.Properties["title"].Value?.ToString()));
        }
        return directorySearchItems;
    }

    /// <summary>
    /// 绑定已有账号。
    /// </summary>
    /// <param name="directoryService"></param>
    /// <param name="person"></param>
    /// <param name="entryObjectGUID"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public async Task<LogonAccount> BindExistsAccount(DirectoryService directoryService, NaturalPerson person, Guid entryObjectGUID)
    {
        using var searchRoot = directoryService.GetRootEntry();
        using DirectorySearcher searcher = new(searchRoot);
        searcher.Filter = $"(objectGUID={entryObjectGUID.ToHexString()})";
        var result = searcher.FindOne();
        if (result == null)
        {
            this.logger?.LogInformation("找不到指定的目录对象。objectGUID是{objectGUID}", entryObjectGUID);
            throw new ArgumentException("Specified directory entry not found.", nameof(entryObjectGUID));
        }
        using var userEntry = result.GetDirectoryEntry();

        LogonAccount logonAccount = new()
        {
            ServiceId = directoryService.Id,
            LogonId = Convert.ToBase64String(entryObjectGUID.ToByteArray()),
            PersonId = person.Id,
        };
        await this.logonAccountStore.CreateAsync(logonAccount);

        //Create external login
        if (directoryService.ExternalLoginProvider != null)
        {
            var providerKey = this.options.GenerateExternalLoginId(directoryService.SAMDomainPart, userEntry);
            var identityResult = await this.naturalPersonManager.AddLoginAsync(person, new Microsoft.AspNetCore.Identity.UserLoginInfo(directoryService.ExternalLoginProvider, providerKey, directoryService.ExternalLoginProviderName));
            if (!identityResult.Succeeded)
            {
                this.logger?.LogError("未能创建外部登录，错误消息：{errors}", identityResult.Errors.Select(p => p.Description));
                throw new InvalidOperationException("未能创建外部登录。");
            }
        }


        return logonAccount;
    }

    /// <summary>
    /// 获取指定用户的账号。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public IEnumerable<LogonAccount> GetLogonAccounts(NaturalPerson person)
    {
        return this.logonAccountStore.Accounts.Where(p => p.PersonId == person.Id);
    }
}
