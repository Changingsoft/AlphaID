using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AlphaID.EntityFramework;

public class NaturalPersonStore : UserStore<NaturalPerson>
{
    public NaturalPersonStore(IDSubjectsDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    { }

    public override Task SetUserNameAsync(NaturalPerson user, string? userName, CancellationToken cancellationToken = default)
    {
        user.UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        return Task.CompletedTask;
    }
}
