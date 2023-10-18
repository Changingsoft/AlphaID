using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AlphaIDEntityFramework.EntityFramework.Identity;

public class NaturalPersonStore : UserStore<NaturalPerson>
{
    public NaturalPersonStore(IDSubjectsDbContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {
    }

    public override Task<IdentityResult> UpdateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.WhenChanged = DateTime.UtcNow;
        return base.UpdateAsync(user, cancellationToken);
    }
}
