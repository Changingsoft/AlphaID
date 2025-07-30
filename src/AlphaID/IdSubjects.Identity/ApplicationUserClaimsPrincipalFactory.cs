using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdSubjects.Identity;

/// <summary>
///    为用户生成声明的工厂。
/// </summary>
/// <typeparam name="TUser">用户类</typeparam>
/// <param name="userManager">The <see cref="UserManager{TUser}"/> to retrieve user information from.</param>
/// <param name="optionsAccessor">The <see cref="RoleManager{TRole}"/> to retrieve a user's roles from.</param>
/// <param name="profileUrlGenerator">个人资料Url生成器。参见<see cref="ProfileUrlGenerator{T}"/>。</param>
public class ApplicationUserClaimsPrincipalFactory<TUser>(UserManager<TUser> userManager,
                                                          IOptions<IdentityOptions> optionsAccessor,
                                                          ProfileUrlGenerator<TUser> profileUrlGenerator)
    : UserClaimsPrincipalFactory<TUser>(userManager, optionsAccessor)
    where TUser : ApplicationUser
{
    /// <inheritdoc />
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
    {
        ClaimsIdentity id = await base.GenerateClaimsAsync(user);
        //string anchor = user.UserName!;
        id.AddClaim(new Claim(JwtClaimTypes.Profile, profileUrlGenerator.GenerateProfileUrl(user).ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.Picture, profileUrlGenerator.GenerateProfilePictureUrl(user).ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.UpdatedAt, user.WhenChanged.ToUnixTimeSeconds().ToString()));
        if (user.Locale != null)
            id.AddClaim(new Claim(JwtClaimTypes.Locale, user.Locale));
        if (user.TimeZone != null)
            id.AddClaim(new Claim(JwtClaimTypes.ZoneInfo, user.TimeZone));
        if (user.Name != null)
            id.AddClaim(new Claim(JwtClaimTypes.Name, user.Name));
        if (user.GivenName != null)
            id.AddClaim(new Claim(JwtClaimTypes.GivenName, user.GivenName));
        if (user.FamilyName != null)
            id.AddClaim(new Claim(JwtClaimTypes.FamilyName, user.FamilyName));
        if (user.MiddleName != null)
            id.AddClaim(new Claim(JwtClaimTypes.MiddleName, user.MiddleName));
        if (user.NickName != null)
            id.AddClaim(new Claim(JwtClaimTypes.NickName, user.NickName));
        if (user.Address != null)
        {
            var oidcAddress = new OidcAddress
            {
                Formatted = $"{user.Address.Street1},{user.Address.Street2},{user.Address.Street3},{user.Address.Locality},{user.Address.Region},{user.Address.PostalCode},{user.Address.Country}",
                StreetAddress = $"{user.Address.Street1},{user.Address.Street2},{user.Address.Street3}",
                Locality = user.Address.Locality,
                Region = user.Address.Region,
                PostalCode = user.Address.PostalCode,
                Country = user.Address.Country
            };
            id.AddClaim(new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(oidcAddress, s_jsonSerializerOptions)));
        }
        if (user.WebSite != null)
            id.AddClaim(new Claim(JwtClaimTypes.WebSite, user.WebSite));
        if (user.DateOfBirth.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.BirthDate, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
        if (user.PhoneNumber != null)
        {
            id.AddClaim(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
            id.AddClaim(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()));
        }
        if (user.Gender.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.Gender, user.Gender.Value.ToString().ToLower()));
        if (user.Email != null)
        {
            id.AddClaim(new Claim(JwtClaimTypes.Email, user.Email));
            id.AddClaim(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));
        }
        id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName ?? user.Email ?? string.Empty));

        return id;
    }

    internal static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
}

/// <summary>
/// 为用户和角色生成声明的工厂。
/// </summary>
/// <typeparam name="TUser">User类</typeparam>
/// <typeparam name="TRole">Role类</typeparam>
/// <param name="userManager"></param>
/// <param name="roleManager"></param>
/// <param name="optionsAccessor"></param>
/// <param name="profileUrlGenerator"></param>
public class ApplicationUserClaimsPrincipalFactory<TUser, TRole>(UserManager<TUser> userManager,
                                                                 RoleManager<TRole> roleManager,
                                                                 IOptions<IdentityOptions> optionsAccessor,
                                                                 ProfileUrlGenerator<TUser> profileUrlGenerator)
    : UserClaimsPrincipalFactory<TUser, TRole>(userManager, roleManager, optionsAccessor)
    where TUser : ApplicationUser
    where TRole : class
{
    /// <inheritdoc />
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
    {
        ClaimsIdentity id = await base.GenerateClaimsAsync(user);
        //string anchor = user.UserName!;
        id.AddClaim(new Claim(JwtClaimTypes.Profile, profileUrlGenerator.GenerateProfileUrl(user).ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.Picture, profileUrlGenerator.GenerateProfilePictureUrl(user).ToString()));
        id.AddClaim(new Claim(JwtClaimTypes.UpdatedAt, user.WhenChanged.ToUnixTimeSeconds().ToString()));
        if (user.Locale != null)
            id.AddClaim(new Claim(JwtClaimTypes.Locale, user.Locale));
        if (user.TimeZone != null)
            id.AddClaim(new Claim(JwtClaimTypes.ZoneInfo, user.TimeZone));
        if (user.Name != null)
            id.AddClaim(new Claim(JwtClaimTypes.Name, user.Name));
        if (user.GivenName != null)
            id.AddClaim(new Claim(JwtClaimTypes.GivenName, user.GivenName));
        if (user.FamilyName != null)
            id.AddClaim(new Claim(JwtClaimTypes.FamilyName, user.FamilyName));
        if (user.MiddleName != null)
            id.AddClaim(new Claim(JwtClaimTypes.MiddleName, user.MiddleName));
        if (user.NickName != null)
            id.AddClaim(new Claim(JwtClaimTypes.NickName, user.NickName));
        if (user.Address != null)
        {
            var oidcAddress = new OidcAddress
            {
                Formatted = $"{user.Address.Street1},{user.Address.Street2},{user.Address.Street3},{user.Address.Locality},{user.Address.Region},{user.Address.PostalCode},{user.Address.Country}",
                StreetAddress = $"{user.Address.Street1},{user.Address.Street2},{user.Address.Street3}",
                Locality = user.Address.Locality,
                Region = user.Address.Region,
                PostalCode = user.Address.PostalCode,
                Country = user.Address.Country
            };
            id.AddClaim(new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(oidcAddress, s_jsonSerializerOptions)));
        }
        if (user.WebSite != null)
            id.AddClaim(new Claim(JwtClaimTypes.WebSite, user.WebSite));
        if (user.DateOfBirth.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.BirthDate, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
        if (user.PhoneNumber != null)
        {
            id.AddClaim(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
            id.AddClaim(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()));
        }
        if (user.Gender.HasValue)
            id.AddClaim(new Claim(JwtClaimTypes.Gender, user.Gender.Value.ToString().ToLower()));
        if (user.Email != null)
        {
            id.AddClaim(new Claim(JwtClaimTypes.Email, user.Email));
            id.AddClaim(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));
        }
        id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName ?? user.Email ?? string.Empty));

        return id;

    }

    internal static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

}