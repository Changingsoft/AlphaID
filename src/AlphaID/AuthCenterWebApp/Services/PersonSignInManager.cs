using IDSubjects;
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
        if (result.Succeeded)
        {
            await this.personManager.AccessSuccededAsync(user, "password");
        }
        return result;
    }
}
