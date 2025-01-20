using IdSubjects;
using Microsoft.Extensions.Options;

namespace AlphaIdPlatform.Identity;
internal class NaturalPersonProfileGenerator(IOptions<OidcProfileUrlOptions> options) : ProfileUrlGenerator<NaturalPerson>(options)
{
    public override Uri GenerateProfileUrl(NaturalPerson user)
    {
        return new Uri(Options.ProfileUrlBase, $"/People/{user.UserName}");
    }

    public override Uri GenerateProfilePictureUrl(NaturalPerson user)
    {
        return new Uri(Options.ProfileUrlBase, $"/People/{user.UserName}/Avatar");
    }
}
