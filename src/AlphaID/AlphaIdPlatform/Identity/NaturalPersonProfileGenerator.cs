using IdSubjects;
using IdSubjects.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlphaIdPlatform.Identity;
internal class NaturalPersonProfileGenerator(IHttpContextAccessor accessor, IOptions<OidcProfileUrlOptions> options) : ProfileUrlGenerator<NaturalPerson>(accessor)
{
    protected OidcProfileUrlOptions Options => options.Value;

    public override Uri GenerateProfileUrl(NaturalPerson user)
    {
        var baseUrl = Options.ProfileUrlBase ?? new Uri($"{Context.Request.Scheme}://{Context.Request.Host}");
        return new Uri(baseUrl, $"/People/{user.UserName}");
    }

    public override Uri GenerateProfilePictureUrl(NaturalPerson user)
    {
        var baseUrl = Options.ProfileUrlBase ?? new Uri($"{Context.Request.Scheme}://{Context.Request.Host}");
        return new Uri(baseUrl, $"/People/{user.UserName}/Avatar");
    }
}
