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
/// <param name="serviceManager"></param>
/// <param name="accountManager"></param>
/// <param name="authenticationStore"></param>
public class NaturalPersonService(
    ApplicationUserManager<ApplicationUser> personManager,
    DirectoryServiceManager? serviceManager,
    DirectoryAccountManager? accountManager,
    IRealNameAuthenticationStore? authenticationStore)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="oldPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser person, string oldPassword, string newPassword)
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
    public async Task<IdentityResult> CreateAsync(ApplicationUser person, string newPassword)
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
    public async Task<IdentityResult> AddPasswordAsync(ApplicationUser person, string newPassword)
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
    public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser person, string inputCode, string inputPassword)
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
    public async Task<IdentityResult?> RemovePasswordAsync(ApplicationUser person)
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    public async Task<IdentityResult?> UpdateAsync(ApplicationUser person)
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
                    person.ApplyTo(userPrincipal);
                    userPrincipal.Save();
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
    public async Task<IdentityResult> DeleteAsync(ApplicationUser person)
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
