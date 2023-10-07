using AlphaIDPlatform;
using IdentityModel;
using IDSubjects;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthCenterWebApp.Services;

public class PersonClaimsPrincipalFactory : UserClaimsPrincipalFactory<NaturalPerson>
{
    private readonly SystemUrlOptions systemUrlOptions;

    public PersonClaimsPrincipalFactory(NaturalPersonManager userManager,
                                        IOptions<IdentityOptions> optionsAccessor,
                                        IOptions<SystemUrlOptions> systemUrlOptions)
        : base(userManager, optionsAccessor)
    {
        this.systemUrlOptions = systemUrlOptions.Value;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(NaturalPerson user)
    {
        var id = await base.GenerateClaimsAsync(user);
        id.AddClaim(new Claim(JwtClaimTypes.Name, user.Name));
        id.AddClaim(new Claim(JwtClaimTypes.Profile, new Uri(this.systemUrlOptions.MyIdUrl, "Profile").ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.Picture, new Uri(this.systemUrlOptions.MyIdUrl, "Profile/Avatar").ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.Locale, "zh-CN")); //todo 从保存用户加载区域选项
        id.AddClaim(new Claim(JwtClaimTypes.ZoneInfo, "Asia/Shanghai")); //todo 从保存用户加载时区信息
        id.AddClaim(new Claim(JwtClaimTypes.UpdatedAt, ((int)(user.WhenChanged - DateTime.UnixEpoch).TotalSeconds).ToString()));
        if (user.FirstName != null)
            id.AddClaim(new Claim(JwtClaimTypes.GivenName, user.FirstName));
        if (user.LastName != null)
            id.AddClaim(new Claim(JwtClaimTypes.FamilyName, user.LastName));

        //todo middle name
        //id.AddClaim(new Claim(JwtClaimTypes.MiddleName, ""));
        //todo nick name
        //id.AddClaim(new Claim(JwtClaimTypes.NickName, ""));
        //todo address
        //id.AddClaim(new Claim(JwtClaimTypes.Address, ""));
        //todo website
        //id.AddClaim(new Claim(JwtClaimTypes.WebSite, ""));

        //Custom claim type phonetic_search_hint.
        if (user.PhoneticSearchHint != null)
            id.AddClaim(new Claim("phonetic_search_hint", user.PhoneticSearchHint));
        if (user.DateOfBirth.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.BirthDate, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
        if (user.Mobile != null)
            id.AddClaim(new Claim(JwtClaimTypes.PhoneNumber, user.Mobile));
        id.AddClaim(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()));
        if (user.Sex.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.Gender, user.Sex.Value.ToString().ToLower()));
        if (user.Email != null)
            id.AddClaim(new Claim(JwtClaimTypes.Email, user.Email));
        id.AddClaim(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));
        return id;
    }
}
