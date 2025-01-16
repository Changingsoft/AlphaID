using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

public class ApplicationUserStore2(IdSubjectsDbContext context, IdentityErrorDescriber? describer = null)
    : UserStore<ApplicationUser>(context, describer), IApplicationUserStore
{
    public async Task<ApplicationUser?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await Context.Set<ApplicationUser>().SingleOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);
    }

    public Task<ApplicationUser?> GetOriginalAsync(ApplicationUser person, CancellationToken cancellationToken)
    {
        //使用跟踪器获取未更改的实体。
        return Task.FromResult(Context.Entry(person).OriginalValues.ToObject() as ApplicationUser);
    }
}