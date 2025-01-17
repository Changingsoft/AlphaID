using System.Security.Claims;
using AlphaIdPlatform;
using AlphaIdPlatform.Identity;
using IdentityModel;
using IdSubjects;
using IdSubjects.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

public class PersonClaimsPrincipalFactory(
    UserManager<NaturalPerson> userManager,
    IOptions<IdSubjectsOptions> optionsAccessor,
    IOptions<SystemUrlInfo> systemUrlOptions) : UserClaimsPrincipalFactory<NaturalPerson>(userManager, optionsAccessor)
{
    private readonly SystemUrlInfo _systemUrl = systemUrlOptions.Value;

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(NaturalPerson user)
    {
        ClaimsIdentity id = await base.GenerateClaimsAsync(user);
        string anchor = user.UserName!;
        id.AddClaim(new Claim(JwtClaimTypes.Profile,
            new Uri(_systemUrl.AuthCenterUrl, "/People/" + anchor).ToString()));
        if (user.ProfilePicture != null)
            id.AddClaim(new Claim(JwtClaimTypes.Picture,
                new Uri(_systemUrl.AuthCenterUrl, $"/People/{anchor}/Avatar").ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.UpdatedAt,
            ((int)(user.WhenChanged - DateTime.UnixEpoch).TotalSeconds).ToString()));
        if (user.Locale != null)
            id.AddClaim(new Claim(JwtClaimTypes.Locale, user.Locale));
        if (user.TimeZone != null)
            id.AddClaim(new Claim(JwtClaimTypes.ZoneInfo, user.TimeZone));
        if (user.HumanName != null)
        {
            id.AddClaim(new Claim(JwtClaimTypes.Name, user.HumanName.FullName));
            if (user.HumanName.GivenName != null)
                id.AddClaim(new Claim(JwtClaimTypes.GivenName, user.HumanName.GivenName));
            if (user.HumanName.Surname != null)
                id.AddClaim(new Claim(JwtClaimTypes.FamilyName, user.HumanName.Surname));
            if (user.HumanName.MiddleName != null)
                id.AddClaim(new Claim(JwtClaimTypes.MiddleName, user.HumanName.MiddleName));
        }
        if (user.NickName != null)
            id.AddClaim(new Claim(JwtClaimTypes.NickName, user.NickName));
        if (user.Address != null)
            //todo 考虑实现地址格式器格式化地址。
            id.AddClaim(new Claim(JwtClaimTypes.Address,
                $"{user.Address.Country},{user.Address.Region},{user.Address.Locality},{user.Address.PostalCode},{user.Address.Street1},{user.Address.Street2},{user.Address.Street3},{user.Address.Receiver},{user.Address.Contact}"));
        if (user.WebSite != null)
            id.AddClaim(new Claim(JwtClaimTypes.WebSite, user.WebSite));
        if (user.DateOfBirth.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.BirthDate, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
        if (user.PhoneNumber != null)
            id.AddClaim(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
        id.AddClaim(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()));
        if (user.Gender.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.Gender, user.Gender.Value.ToString().ToLower()));
        if (user.Email != null)
            id.AddClaim(new Claim(JwtClaimTypes.Email, user.Email));
        id.AddClaim(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName ?? user.Email ?? string.Empty));

        //Custom claim type SearchHint.
        if (user.SearchHint != null)
            id.AddClaim(new Claim(AlphaIdJwtClaimTypes.SearchHint, user.SearchHint));
        return id;
    }
}