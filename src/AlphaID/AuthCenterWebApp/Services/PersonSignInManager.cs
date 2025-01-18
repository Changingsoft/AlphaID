using System.Security.Claims;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

/// <summary>
///     自然人登录管理器。继承自<see cref="SignInManager{TUser}"></see>
/// </summary>
public class PersonSignInManager(
    ApplicationUserManager<NaturalPerson> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<NaturalPerson> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<NaturalPerson>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<NaturalPerson> confirmation) : SignInManager<NaturalPerson>(userManager,
    contextAccessor,
    claimsFactory,
    optionsAccessor,
    logger,
    schemes,
    confirmation)
{
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    ///     重写。当使用密码登录成功时，检查是否必须更改密码，并通知PersonManager记录登录成功信息。
    /// </summary>
    /// <param name="user">自然人</param>
    /// <param name="password">密码</param>
    /// <param name="lockoutOnFailure">指示登录失败时是否锁定。</param>
    /// <returns></returns>
    public override async Task<SignInResult> CheckPasswordSignInAsync(NaturalPerson user,
        string password,
        bool lockoutOnFailure)
    {
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
                return new PersonSignInResult { MustChangePassword = true };
            }

        await userManager.AccessSuccededAsync(user, "password");
        return result;
    }

    private static ClaimsPrincipal GenerateMustChangePasswordPrincipal(NaturalPerson person)
    {
        var identity = new ClaimsIdentity(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, person.Id));
        return new ClaimsPrincipal(identity);
    }
}