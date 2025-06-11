using System.DirectoryServices;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Identity;
using System.DirectoryServices.AccountManagement;
using IdSubjects;
using IdSubjects.RealName;

namespace AlphaIdPlatform.Identity;

/// <summary>
/// 自然人服务。
/// </summary>
/// <param name="userManager"></param>
/// <param name="serviceManager"></param>
/// <param name="accountManager"></param>
/// <param name="authenticationStore"></param>
public class NaturalPersonService(
    ApplicationUserManager<NaturalPerson> userManager,
    DirectoryServiceManager? serviceManager,
    DirectoryAccountManager<NaturalPerson>? accountManager,
    IRealNameAuthenticationStore? authenticationStore)
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
        var result = await userManager.ChangePasswordAsync(person, oldPassword, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }
        accountManager?.SetAllPassword(person, newPassword, !person.PasswordLastSet.HasValue);

        return result;
    }

    /// <summary>
    /// 创建自然人。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateAsync(NaturalPerson user, string newPassword)
    {
        var result = await userManager.CreateAsync(user, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }

        // 创建目录账号
        if (accountManager != null)
        {
            await accountManager.CreateDirectoryAccounts(user, newPassword);
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
        var result = await userManager.AddPasswordAsync(person, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }
        accountManager?.SetAllPassword(person, newPassword, !person.PasswordLastSet.HasValue);

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
        var result = await userManager.ResetPasswordAsync(person, inputCode, inputPassword);
        if (!result.Succeeded)
        {
            return result;
        }
        accountManager?.SetAllPassword(person, inputPassword, !person.PasswordLastSet.HasValue);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="newPassword"></param>
    /// <param name="mustChangePassword"></param>
    /// <param name="unlockUser"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ResetPasswordAsync(NaturalPerson person, string newPassword, bool mustChangePassword, bool unlockUser)
    {
        var result = await userManager.ResetPasswordAsync(person, newPassword, mustChangePassword, unlockUser);
        if (!result.Succeeded)
        {
            return result;
        }
        accountManager?.SetAllPassword(person, newPassword, !person.PasswordLastSet.HasValue);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> RemovePasswordAsync(NaturalPerson person)
    {
        var result = await userManager.RemovePasswordAsync(person);
        if (!result.Succeeded)
        {
            return result;
        }
        accountManager?.SetAllPassword(person, null, !person.PasswordLastSet.HasValue);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> UpdateAsync(NaturalPerson person)
    {
        var result = await userManager.UpdateAsync(person);
        if (!result.Succeeded)
        {
            return result;
        }
        if (serviceManager != null && accountManager != null)
        {
            accountManager.ApplyUpdates(person);
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
        var result = await userManager.DeleteAsync(person);
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

    /// <summary>
    /// 设置电子邮件。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetEmailAsync(NaturalPerson person, string? email)
    {
        var result = await userManager.SetEmailAsync(person, email);
        if (!result.Succeeded)
            return result;

        //todo 更新目录账号的邮件地址。
        return result;
    }

    /// <summary>
    /// 设置手机号码。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetPhoneNumberAsync(NaturalPerson person, string? phoneNumber)
    {
        var result = await userManager.SetPhoneNumberAsync(person, phoneNumber);
        if (!result.Succeeded)
            return result;
        //todo 更新目录账号的手机号码。
        return result;
    }
}
