using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="userManager"></param>
/// <param name="contextAccessor"></param>
/// <param name="claimsFactory"></param>
/// <param name="optionsAccessor"></param>
/// <param name="logger"></param>
/// <param name="schemes"></param>
/// <param name="confirmation"></param>
public class ApplicationUserSignInManager<T>(ApplicationUserManager<T> userManager,
                                    IHttpContextAccessor contextAccessor,
                                    IUserClaimsPrincipalFactory<T> claimsFactory,
                                    IOptions<IdentityOptions> optionsAccessor,
                                    ILogger<SignInManager<T>> logger,
                                    IAuthenticationSchemeProvider schemes,
                                    IUserConfirmation<T> confirmation) : SignInManager<T>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) where T : ApplicationUser
{
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 重写。当使用密码登录成功时，检查是否必须更改密码，并通知PersonManager记录登录成功信息。
    /// </summary>
    /// <param name="user">自然人</param>
    /// <param name="password">密码</param>
    /// <param name="lockoutOnFailure">指示登录失败时是否锁定。</param>
    /// <returns></returns>
    public override async Task<SignInResult> CheckPasswordSignInAsync(T user,
        string password,
        bool lockoutOnFailure)
    {
        if (!user.Enabled)
        {
            await userManager.AccessFailedAsync(user);
            return SignInResult.NotAllowed;
        }

        SignInResult result = await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        if (!result.Succeeded) return result;

        //如果密码验证成功，则检查是否需要强制修改密码。
        if (userManager.PasswordLifetime.EnablePassExpires)
            if (user.PasswordLastSet == null || user.PasswordLastSet.Value <
                TimeProvider.GetUtcNow().AddDays(0 - userManager.PasswordLifetime.PasswordExpiresDay))
            {
                ClaimsPrincipal principal = GenerateMustChangePasswordPrincipal(user);
                await base.SignOutAsync();
                await Context.SignInAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme,
                    principal);
                return new ApplicationUserSignInResult { MustChangePassword = true };
            }

        await userManager.AccessSuccededAsync(user, "password");
        return result;
    }

    private static ClaimsPrincipal GenerateMustChangePasswordPrincipal(T person)
    {
        var identity = new ClaimsIdentity(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, person.Id));
        return new ClaimsPrincipal(identity);
    }
}
