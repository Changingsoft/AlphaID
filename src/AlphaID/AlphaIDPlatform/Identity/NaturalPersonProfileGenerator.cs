using IdSubjects;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaIdPlatform.Identity;
internal class NaturalPersonProfileGenerator(IOptions<OidcProfileUrlOptions> options) : ProfileUrlGenerator<NaturalPerson>(options)
{
    public override Uri GenerateProfileUrl(NaturalPerson user)
    {
        return new Uri(options.Value.ProfileUrlBase, $"/People/{user.UserName}");
    }

    public override Uri GenerateProfilePictureUrl(NaturalPerson user)
    {
        return new Uri(options.Value.ProfileUrlBase, $"/People/{user.UserName}/Avatar");
    }
}
