using Duende.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Logon Account Manager.
/// </summary>
/// <remarks>
/// Init.
/// </remarks>
/// <param name="userManager"></param>
/// <param name="store"></param>
/// <param name="directoryServiceManager"></param>
/// <param name="subjectGenerators"></param>
/// <param name="logger"></param>
public class DirectoryAccountManager<T>(
    UserManager<T> userManager,
    IDirectoryAccountStore store,
    DirectoryServiceManager directoryServiceManager,
    IEnumerable<ISubjectGenerator> subjectGenerators,
    ILogger<DirectoryAccountManager<T>>? logger = null)
where T : ApplicationUser
{
    /// <summary>
    /// 为用户在指定的目录服务中创建一个目录账号。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <param name="service">目录服务。</param>
    /// <param name="userPassword">新密码。</param>
    /// <returns></returns>
    public virtual async Task<DirectoryAccount> CreateDirectoryAccount(T user,
        DirectoryService service,
        string? userPassword = null)
    {
        using PrincipalContext context = PrincipalContextHelper.GetUserContainerContext(service);
        using UserPrincipal principal = new(context);
        principal.SamAccountName = user.UserName;
        if (user.Email != null)
            principal.EmailAddress = user.Email;
        if (user.GivenName != null)
            principal.GivenName = user.GivenName;
        if (user.FamilyName != null)
            principal.Surname = user.FamilyName;
        if (user.MiddleName != null)
            principal.MiddleName = user.MiddleName;
        if (user.Name != null)
            principal.DisplayName = user.Name;

        var entry = (DirectoryEntry)principal.GetUnderlyingObject();
        if (user.PhoneNumber != null)
            entry.Properties["mobile"].Value = user.PhoneNumber;
        entry.Properties["sAMAccountName"].Value = user.UserName;
        entry.CommitChanges();
        principal.Save();

        if (userPassword != null)
        {
            principal.SetPassword(userPassword);
            //设置密码不需要强制更改密码。
            entry.Properties["pwdLastSet"][0] = -1;
        }
        principal.Save();

        DirectoryAccount account = new(service, user.Id)
        {
            ObjectId = principal.Guid.ToString()!
        };
        await store.CreateAsync(account);

        //Create external login
        if (service.ExternalLoginProvider != null)
        {
            ClaimsPrincipal p = new(new ClaimsIdentity(
            [
                new(JwtClaimTypes.Subject, $"{service.SamDomainPart}\\{principal.SamAccountName}"),
                new(JwtClaimTypes.ClientId, service.ExternalLoginProvider.RegisteredClientId)
            ]));
            ISubjectGenerator generator =
                service.ExternalLoginProvider.SubjectGenerator != null
                    ? subjectGenerators.First(s => s.GetType().FullName == service.ExternalLoginProvider.SubjectGenerator)
                    : subjectGenerators.First();
            string providerKey = generator.Generate(p);
            IdentityResult identityResult = await userManager.AddLoginAsync(user,
                new UserLoginInfo(service.ExternalLoginProvider.Name, providerKey,
                    service.ExternalLoginProvider.DisplayName));
            if (!identityResult.Succeeded)
            {
                logger?.LogError("未能创建外部登录，错误消息：{errors}", identityResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException("未能创建外部登录。");
            }
        }

        return account;
    }

    /// <summary>
    /// 为指定的用户在具有<see cref="DirectoryService.AutoCreateAccount"></see>为true的目录服务中创建关联目录账号。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <param name="userPassword">新密码。</param>
    /// <returns>返回已创建的目录服务账户集合。</returns>
    public async Task<IEnumerable<DirectoryAccount>> CreateDirectoryAccounts(T user, string? userPassword = null)
    {
        IEnumerable<DirectoryService> services = directoryServiceManager.Services.Where(p => p.AutoCreateAccount);
        List<DirectoryAccount> accounts = [];
        foreach (DirectoryService service in services)
        {
            try
            {
                accounts.Add(await CreateDirectoryAccount(user, service, userPassword));
            }
            catch (Exception e)
            {
                logger?.LogError(e, "在目录服务{serviceName}中创建账号时出错。", service.Name);
            }
        }
        return accounts;
    }

    /// <summary>
    /// Updates the specified directory account with the information provided by the user object.
    /// </summary>
    /// <remarks>This method updates various attributes of the directory account, including username, email
    /// address, name components, and phone number, based on the values provided in the <paramref name="user"/> object.
    /// If any of these values are null, the corresponding attribute in the directory account will remain
    /// unchanged.</remarks>
    /// <param name="user">The user object containing updated information to apply to the directory account. Cannot be null.</param>
    /// <param name="account">The directory account to be updated. Cannot be null.</param>
    /// <returns>An <see cref="IdOperationResult"/> indicating the success or failure of the operation. Returns <see
    /// cref="IdOperationResult.Success"/> if the update is applied successfully. Returns <see
    /// cref="IdOperationResult.Failed"/> if the specified directory account cannot be found.</returns>
    public virtual IdOperationResult ApplyUpdate(T user, DirectoryAccount account)
    {
        //更新目录账号信息
        using var context = PrincipalContextHelper.GetRootContext(account.DirectoryService);
        UserPrincipal? principal = UserPrincipal.FindByIdentity(context, account.ObjectId);
        if (principal == null)
        {
            return IdOperationResult.Failed("找不到指定的目录账号。");
        }
        principal.SamAccountName = user.UserName;
        if (user.Email != null)
            principal.EmailAddress = user.Email;
        if (user.GivenName != null)
            principal.GivenName = user.GivenName;
        if (user.FamilyName != null)
            principal.Surname = user.FamilyName;
        if (user.MiddleName != null)
            principal.MiddleName = user.MiddleName;
        if (user.Name != null)
            principal.DisplayName = user.Name;
        var entry = (DirectoryEntry)principal.GetUnderlyingObject();
        if (user.PhoneNumber != null)
            entry.Properties["mobile"].Value = user.PhoneNumber;
        entry.Properties["sAMAccountName"].Value = user.UserName;
        entry.CommitChanges();
        principal.Save();
        return IdOperationResult.Success;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual IdOperationResult ApplyUpdates(T user)
    {
        var accounts = GetLogonAccounts(user);
        foreach (var account in accounts)
        {
            ApplyUpdate(user, account);
        }
        return IdOperationResult.Success;
    }

    /// <summary>
    /// 链接用户到目录账户。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="service"></param>
    /// <param name="entryObjectGuid"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<DirectoryAccount> LinkExistsAccount(T user, DirectoryService service, string entryObjectGuid)
    {
        using var context = PrincipalContextHelper.GetRootContext(service);
        UserPrincipal? userPrincipal = UserPrincipal.FindByIdentity(context, entryObjectGuid);
        if (userPrincipal == null)
        {
            throw new ArgumentException("找不到指定的目录对象。", nameof(entryObjectGuid));
        }
        DirectoryAccount account = new(service, user.Id)
        {
            ObjectId = userPrincipal.Guid.ToString()!
        };
        await store.CreateAsync(account);
        return account;
    }

    /// <summary>
    /// 解除关联目录账号的绑定关系。解除关联后，目录账户不会被删除，仍可以通过LinkExistsAccount方法重新关联。
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public virtual async Task<IdOperationResult> UnlinkAccount(DirectoryAccount account)
    {
        if (account.DirectoryService.ExternalLoginProvider != null)
        {
            T? person = userManager.Users.FirstOrDefault(p => p.Id == account.UserId);
            if (person == null) return IdOperationResult.Failed("找不到指定的Person。");
            //删除外部登录
            IdentityResult identityResult = await userManager.RemoveLoginAsync(person, account.DirectoryService.ExternalLoginProvider.Name, account.ObjectId);
            if (!identityResult.Succeeded)
            {
                logger?.LogError("未能删除外部登录，错误消息：{errors}", identityResult.Errors.Select(p => p.Description));
            }
        }

        //删除目录账号
        await store.DeleteAsync(account);
        return IdOperationResult.Success;
    }

    /// <summary>
    /// 为账户设置密码。
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <param name="mustChangePassword"></param>
    /// <returns></returns>
    public IdOperationResult SetPassword(DirectoryAccount account, string? password, bool mustChangePassword = false)
    {
        using var context = PrincipalContextHelper.GetRootContext(account.DirectoryService);
        UserPrincipal? principal = UserPrincipal.FindByIdentity(context, account.ObjectId);
        if (principal == null)
        {
            return IdOperationResult.Failed("找不到指定的目录账号。");
        }
        try
        {
            principal.SetPassword(password);
            if (mustChangePassword)
            {
                var entry = (DirectoryEntry)principal.GetUnderlyingObject();
                entry.Properties["pwdLastSet"][0] = 0;
            }
            principal.Save();
            return IdOperationResult.Success;
        }
        catch (Exception e)
        {
            logger?.LogError(e, "设置密码时出错。");
            return IdOperationResult.Failed("设置密码时出错。");
        }
    }

    public IdOperationResult SetAllPassword(T user, string? password, bool mustChangePassword = false)
    {
        var accounts = GetLogonAccounts(user);
        foreach (var account in accounts)
        {
            IdOperationResult result = SetPassword(account, password, mustChangePassword);
            if (!result.Succeeded)
            {
                return result; // 返回第一个失败的结果
            }
        }
        return IdOperationResult.Success;
    }

    /// <summary>
    /// 获取指定用户的所有目录账号。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public IEnumerable<DirectoryAccount> GetLogonAccounts(T user)
    {
        return store.Accounts.Where(p => p.UserId == user.Id);
    }
}