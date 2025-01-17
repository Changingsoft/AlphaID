using System.Security.Claims;
using AlphaIdPlatform;
using AlphaIdPlatform.Identity;
using IdentityModel;
using IdSubjects;
using IdSubjects.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

/// <summary>
/// 自然人声明主体工厂。已重写生成声明的方法，添加自定义声明 SearchHint。
/// </summary>
/// <param name="userManager"></param>
/// <param name="optionsAccessor"></param>
/// <param name="profileUrlGenerator"></param>
public class NaturalPersonClaimsPrincipalFactory(UserManager<NaturalPerson> userManager,
                                          IOptions<IdentityOptions> optionsAccessor,
                                          ProfileUrlGenerator<NaturalPerson> profileUrlGenerator) 
    : ApplicationUserClaimsPrincipalFactory<NaturalPerson>(userManager, optionsAccessor, profileUrlGenerator)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(NaturalPerson user)
    {
        ClaimsIdentity id = await base.GenerateClaimsAsync(user);

        //Custom claim type SearchHint.
        if (user.SearchHint != null)
            id.AddClaim(new Claim(AlphaIdJwtClaimTypes.SearchHint, user.SearchHint));
        return id;
    }
}