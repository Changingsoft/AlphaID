using IdSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

public class PersonSignInManager : SignInManager<NaturalPerson>
{
    private readonly NaturalPersonManager personManager;

    public PersonSignInManager(NaturalPersonManager userManager,
                               IHttpContextAccessor contextAccessor,
                               IUserClaimsPrincipalFactory<NaturalPerson> claimsFactory,
                               IOptions<IdentityOptions> optionsAccessor,
                               ILogger<SignInManager<NaturalPerson>> logger,
                               IAuthenticationSchemeProvider schemes,
                               IUserConfirmation<NaturalPerson> confirmation)
        : base(userManager,
               contextAccessor,
               claimsFactory,
               optionsAccessor,
               logger,
               schemes,
               confirmation)
    {
        this.personManager = userManager;
    }

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 重写。当使用密码登录成功时，记录登录成功信息。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="lockoutOnFailure"></param>
    /// <returns></returns>
    public override async Task<SignInResult> CheckPasswordSignInAsync(NaturalPerson user, string password, bool lockoutOnFailure)
    {
        var result = await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        if (!result.Succeeded)
        {
            return result;
        }

        //如果密码验证成功，则检查是否需要强制修改密码。
        if (!this.personManager.Options.Password.EnablePassExpires)
        {
            return result;
        }

        if (user.PasswordLastSet == null || user.PasswordLastSet.Value < this.TimeProvider.GetUtcNow().AddDays(0 - this.personManager.Options.Password.PasswordExpiresDay))
        {
            return new PersonSignInResult { MustChangePassword = true };
        }
        await this.personManager.AccessSuccededAsync(user, "password");
        return result;
    }
}
