using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AlphaIDEntityFramework.EntityFramework.Identity;

public class NaturalPersonStore : UserStore<NaturalPerson>
{
    public NaturalPersonStore(IDSubjectsDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    { }
}
