using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Logon Account Manager.
/// </summary>
/// <remarks>
/// Init.
/// </remarks>
/// <param name="applicationUserManager"></param>
/// <param name="directoryAccountStore"></param>
/// <param name="subjectGenerators"></param>
/// <param name="logger"></param>
public class DirectoryAccountManager<T>(
    UserManager<T> applicationUserManager,
    IDirectoryAccountStore directoryAccountStore,
    IEnumerable<ISubjectGenerator> subjectGenerators,
    ILogger<DirectoryAccountManager<T>>? logger = null)
where T : ApplicationUser
{
    /// <summary>
    /// Create account.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public async Task<IdOperationResult> CreateAsync(DirectoryAccount account)
    {
        T? person = await applicationUserManager.FindByIdAsync(account.PersonId);
        if (person == null) return IdOperationResult.Failed("找不到指定的Person。");
        using PrincipalContext context = account.DirectoryServiceDescriptor.GetUserContainerContext();
        UserPrincipal newAccount = new(context)
        {
            SamAccountName = person.UserName
        };
        person.ApplyTo(newAccount);

        newAccount.Save();

        try
        {
            //userEntry.Properties["userPrincipalName"].Value = $"{request.UpnLeftPart}@{directoryService.UpnSuffix}";
            //if (request.PinyinSurname != null)
            //    userEntry.Properties["msDS-PhoneticLastName"].Value = request.PinyinSurname;
            //if (request.PinyinGivenName != null)
            //    userEntry.Properties["msDS-PhoneticFirstName"].Value = request.PinyinGivenName;
            //var pinyin = $"{request.PinyinSurname} {request.PinyinGivenName}".Trim();
            //if (!string.IsNullOrEmpty(pinyin))
            //    userEntry.Properties["msDS-PhoneticDisplayName"].Value = pinyin;

            //Set Init Password
            //const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
            //const long ADS_OPTION_PASSWORD_METHOD = 7;
            //const int ADS_PASSWORD_ENCODE_CLEAR = 1;
            //userEntry.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_PORTNUMBER, 389 });
            //userEntry.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR });

            //set User Account Control Flag
            //userEntry.Properties["msDS-UserAccountDisabled"].Clear();  // normal account


            account.ObjectId = newAccount.Guid.ToString()!;

            await directoryAccountStore.CreateAsync(account);

            //Create external login
            if (account.DirectoryServiceDescriptor.ExternalLoginProvider != null)
            {
                ClaimsPrincipal p = new(new ClaimsIdentity(
                [
                    new(JwtClaimTypes.Subject,
                        $"{account.DirectoryServiceDescriptor.SamDomainPart}\\{newAccount.SamAccountName}"),
                    new(JwtClaimTypes.ClientId,
                        account.DirectoryServiceDescriptor.ExternalLoginProvider.RegisteredClientId)
                ]));

                ISubjectGenerator generator =
                    account.DirectoryServiceDescriptor.ExternalLoginProvider.SubjectGenerator != null
                        ? subjectGenerators.First(s =>
                            s.GetType().FullName == account.DirectoryServiceDescriptor.ExternalLoginProvider
                                .SubjectGenerator)
                        : subjectGenerators.First();

                string providerKey = generator.Generate(p);
                IdentityResult identityResult = await applicationUserManager.AddLoginAsync(person,
                    new UserLoginInfo(account.DirectoryServiceDescriptor.ExternalLoginProvider.Name, providerKey,
                        account.DirectoryServiceDescriptor.ExternalLoginProvider.DisplayName));
                if (!identityResult.Succeeded)
                {
                    logger?.LogError("未能创建外部登录，错误消息：{errors}", identityResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException("未能创建外部登录。");
                }
            }

            return IdOperationResult.Success;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "向目录服务创建用户账户时出错");
            throw;
        }
    }

    /// <summary>
    /// Search from directory service.
    /// </summary>
    /// <param name="directoryServiceDescriptor"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public IEnumerable<DirectorySearchItem> Search(DirectoryServiceDescriptor directoryServiceDescriptor, string filter)
    {
        using DirectoryEntry searchRoot = directoryServiceDescriptor.GetRootEntry();
        using DirectorySearcher searcher = new(searchRoot);
        searcher.Filter = filter;
        SearchResultCollection results = searcher.FindAll();
        HashSet<DirectorySearchItem> directorySearchItems = [];
        foreach (SearchResult searchResult in results)
        {
            using DirectoryEntry entry = searchResult.GetDirectoryEntry();
            directorySearchItems.Add(new DirectorySearchItem(entry.Properties["name"].Value!.ToString()!,
                entry.Properties["sAMAccountName"].Value?.ToString(),
                entry.Properties["userPrincipalName"].Value?.ToString()!,
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
    /// <param name="account"></param>
    /// <param name="entryObjectGuid"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public async Task<IdOperationResult> BindExistsAccount(DirectoryAccount account,
        string entryObjectGuid)
    {
        T? person = await applicationUserManager.FindByIdAsync(account.PersonId);
        if (person == null) return IdOperationResult.Failed("找不到指定的Person。");

        using PrincipalContext context = account.DirectoryServiceDescriptor.GetRootContext();
        UserPrincipal user = UserPrincipal.FindByIdentity(context, entryObjectGuid);
        if (user == null)
        {
            logger?.LogInformation("找不到指定的目录对象。objectGUID是{objectGUID}", entryObjectGuid);
            return IdOperationResult.Failed("找不到指定的目录对象。");
        }

        person.ApplyTo(user);
        user.Save();
        account.ObjectId = entryObjectGuid;
        await directoryAccountStore.CreateAsync(account);

        //Create external login
        if (account.DirectoryServiceDescriptor.ExternalLoginProvider != null)
        {
            string anchorValue = user.Guid?.ToString() ?? user.Name;
            if (user.SamAccountName != null)
                anchorValue = $"{account.DirectoryServiceDescriptor.SamDomainPart}\\{user.SamAccountName}";
            ClaimsPrincipal principal = new(new ClaimsIdentity(
            [
                new(JwtClaimTypes.Subject, anchorValue),
                new(JwtClaimTypes.ClientId, account.DirectoryServiceDescriptor.ExternalLoginProvider.RegisteredClientId)
            ]));

            ISubjectGenerator generator =
                account.DirectoryServiceDescriptor.ExternalLoginProvider.SubjectGenerator != null
                    ? subjectGenerators.First(s =>
                        s.GetType().FullName ==
                        account.DirectoryServiceDescriptor.ExternalLoginProvider.SubjectGenerator)
                    : subjectGenerators.First();
            string providerKey = generator.Generate(principal);
            IdentityResult identityResult = await applicationUserManager.AddLoginAsync(person,
                new UserLoginInfo(account.DirectoryServiceDescriptor.ExternalLoginProvider.Name, providerKey,
                    account.DirectoryServiceDescriptor.ExternalLoginProvider.DisplayName));
            if (!identityResult.Succeeded)
            {
                logger?.LogError("未能创建外部登录，错误消息：{errors}", identityResult.Errors.Select(p => p.Description));
                throw new InvalidOperationException("未能创建外部登录。");
            }
        }


        return IdOperationResult.Success;
    }

    /// <summary>
    /// 获取指定用户的账号。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public IEnumerable<DirectoryAccount> GetLogonAccounts(ApplicationUser person)
    {
        return directoryAccountStore.Accounts.Where(p => p.PersonId == person.Id);
    }
}