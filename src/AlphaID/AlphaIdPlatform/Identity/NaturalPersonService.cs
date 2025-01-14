using IdSubjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Identity;
using System.DirectoryServices.AccountManagement;
using IdSubjects.RealName;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Extensions.Logging;

namespace AlphaIdPlatform.Identity;

/// <summary>
/// 自然人服务。
/// </summary>
/// <param name="personManager"></param>
/// <param name="accountManager"></param>
public class NaturalPersonService(
    NaturalPersonManager personManager,
    DirectoryServiceManager? serviceManager,
    DirectoryAccountManager? accountManager,
    IRealNameAuthenticationStore? authenticationStore,
    ILogger<NaturalPersonService>? logger)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="oldPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ChangePasswordAsync(NaturalPerson person, string oldPassword, string newPassword)
    {
        var result = await personManager.ChangePasswordAsync(person, oldPassword, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }
        if (serviceManager != null && accountManager != null)
        {
            var directoryServices = accountManager.GetLogonAccounts(person);
            foreach (var account in directoryServices)
            {
                account.SetPassword(newPassword, !person.PasswordLastSet.HasValue);
            }
        }

        return result;
    }

    /// <summary>
    /// 创建自然人。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateAsync(NaturalPerson person, string newPassword)
    {
        var result = await personManager.CreateAsync(person, newPassword);
        if(!result.Succeeded)
        {
            return result;
        }

        // 创建目录账号
        if (serviceManager != null && accountManager != null)
        {
            var directoryServices = serviceManager.Services.Where(s => s.AutoCreateAccount);
            foreach (var directoryService in directoryServices)
            {
                var account = new DirectoryAccount(directoryService, person.Id);
                await accountManager.CreateAsync(account);
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddPasswordAsync(NaturalPerson person, string newPassword)
    {
        var result = await personManager.AddPasswordAsync(person, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }
        if (serviceManager != null && accountManager != null)
        {
            var directoryServices = accountManager.GetLogonAccounts(person);
            foreach (var account in directoryServices)
            {
                account.SetPassword(newPassword, !person.PasswordLastSet.HasValue);
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="inputCode"></param>
    /// <param name="inputPassword"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ResetPasswordAsync(NaturalPerson person, string inputCode, string inputPassword)
    {
        var result = await personManager.ResetPasswordAsync(person, inputCode, inputPassword);
        if (!result.Succeeded)
        {
            return result;
        }
        if (serviceManager != null && accountManager != null)
        {
            var directoryServices = accountManager.GetLogonAccounts(person);
            foreach (var account in directoryServices)
            {
                account.SetPassword(inputPassword, !person.PasswordLastSet.HasValue);
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> RemovePasswordAsync(NaturalPerson person)
    {
        var result = await personManager.RemovePasswordAsync(person);
        if (!result.Succeeded)
        {
            return result;
        }
        if (serviceManager != null && accountManager != null)
        {
            var directoryServices = accountManager.GetLogonAccounts(person);
            foreach (var account in directoryServices)
            {
                account.SetPassword(null, !person.PasswordLastSet.HasValue);
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> UpdateAsync(NaturalPerson person)
    {
        var result = await personManager.UpdateAsync(person);
        if (!result.Succeeded)
        {
            return result;
        }
        if (serviceManager != null && accountManager != null)
        {
            var directoryServices = accountManager.GetLogonAccounts(person);
            foreach (var account in directoryServices)
            {
                UserPrincipal? userPrincipal = account.GetUserPrincipal();
                if (userPrincipal != null)
                {
                    person.Apply(userPrincipal);
                    userPrincipal.Save();
                }
            }
        }

        if (authenticationStore != null)
        {
            IQueryable<RealNameAuthentication> personAuthentications = authenticationStore.FindByPerson(person);
            IEnumerable<RealNameAuthentication> _pendingAuthentications =
                [.. personAuthentications.Where(a => !a.Applied)];
            if (_pendingAuthentications.Any())
            {
                foreach (RealNameAuthentication authentication in _pendingAuthentications)
                {
                    authentication.ApplyToPerson(person);
                    logger?.LogDebug("拦截器使用{authentication}覆盖了{person}的信息。", authentication, person);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public async Task<IdentityResult> DeleteAsync(NaturalPerson person)
    {
        var result = await personManager.DeleteAsync(person);
        if (!result.Succeeded)
        {
            return result;
        }

        if (authenticationStore != null)
        {
            await authenticationStore.DeleteByPersonIdAsync(person.Id);
        }

        return result;
    }
}
