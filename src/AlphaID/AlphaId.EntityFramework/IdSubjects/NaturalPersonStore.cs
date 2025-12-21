using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

public class NaturalPersonStore(AlphaIdIdentityDbContext context, IdentityErrorDescriber? describer = null)
    : UserStore<NaturalPerson>(context, describer), IApplicationUserStore<NaturalPerson>
{
    public async Task<NaturalPerson?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await Context.Set<NaturalPerson>().SingleOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);
    }
}