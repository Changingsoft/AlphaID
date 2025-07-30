using IdSubjects;
using IdSubjects.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlphaIdPlatform.Identity;
internal class NaturalPersonProfileGenerator(IHttpContextAccessor accessor, IOptions<OidcProfileUrlOptions> options) : ProfileUrlGenerator<NaturalPerson>(accessor, options)
{
    public override Uri GenerateProfileUrl(NaturalPerson user)
    {
        var baseUrl = new Uri($"{Context.Request.Scheme}://{Context.Request.Host}");
        return new Uri(baseUrl, $"/People/{user.UserName}");
    }

    public override Uri GenerateProfilePictureUrl(NaturalPerson user)
    {
        var baseUrl = new Uri($"{Context.Request.Scheme}://{Context.Request.Host}");
        return new Uri(baseUrl, $"/People/{user.UserName}/Avatar");
    }
}
