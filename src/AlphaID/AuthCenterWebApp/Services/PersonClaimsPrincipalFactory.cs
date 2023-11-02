using AlphaIDPlatform;
using IdentityModel;
using IDSubjects;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthCenterWebApp.Services;

public class PersonClaimsPrincipalFactory : UserClaimsPrincipalFactory<NaturalPerson>
{
    private readonly SystemUrlInfo systemUrl;

    public PersonClaimsPrincipalFactory(NaturalPersonManager userManager,
                                        IOptions<IdentityOptions> optionsAccessor,
                                        IOptions<SystemUrlInfo> systemUrlOptions)
        : base(userManager, optionsAccessor)
    {
        this.systemUrl = systemUrlOptions.Value;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(NaturalPerson user)
    {
        var id = await base.GenerateClaimsAsync(user);
        id.AddClaim(new Claim(JwtClaimTypes.Name, user.Name));
        var userAnchor = user.UserName ?? user.Id;
        id.AddClaim(new Claim(JwtClaimTypes.Profile, new Uri(this.systemUrl.AuthCenterUrl, "/People/" + userAnchor).ToString()));
        if (user.Avatar != null)
            id.AddClaim(new Claim(JwtClaimTypes.Picture, new Uri(this.systemUrl.AuthCenterUrl, $"/People/{userAnchor}/Avatar").ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.UpdatedAt, ((int)(user.WhenChanged - DateTime.UnixEpoch).TotalSeconds).ToString()));
        if (user.Locale != null)
            id.AddClaim(new Claim(JwtClaimTypes.Locale, user.Locale));
        if (user.TimeZone != null)
            id.AddClaim(new Claim(JwtClaimTypes.ZoneInfo, user.TimeZone));
        if (user.FirstName != null)
            id.AddClaim(new Claim(JwtClaimTypes.GivenName, user.FirstName));
        if (user.LastName != null)
            id.AddClaim(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        if (user.MiddleName != null)
            id.AddClaim(new Claim(JwtClaimTypes.MiddleName, user.MiddleName));
        if (user.NickName != null)
            id.AddClaim(new Claim(JwtClaimTypes.NickName, user.NickName));
        if (user.Address != null)
            id.AddClaim(new Claim(JwtClaimTypes.Address, $"{user.Address.Country},{user.Address.State},{user.Address.City},{user.Address.PostalCode},{user.Address.Street1},{user.Address.Street2},{user.Address.Street3},{user.Address.Company},{user.Address.Receiver},{user.Address.Contact}"));
        if (user.WebSite != null)
            id.AddClaim(new Claim(JwtClaimTypes.WebSite, user.WebSite));

        //Custom claim type phonetic_search_hint.
        if (user.PhoneticSearchHint != null)
            id.AddClaim(new Claim("phonetic_search_hint", user.PhoneticSearchHint));
        if (user.DateOfBirth.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.BirthDate, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
        if (user.PhoneNumber != null)
            id.AddClaim(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
        id.AddClaim(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()));
        if (user.Sex.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.Gender, user.Sex.Value.ToString().ToLower()));
        if (user.Email != null)
            id.AddClaim(new Claim(JwtClaimTypes.Email, user.Email));
        id.AddClaim(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));
        if (user.UserName != null)
            id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));
        return id;
    }
}
