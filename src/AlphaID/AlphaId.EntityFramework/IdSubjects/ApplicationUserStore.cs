using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

public class ApplicationUserStore(IdSubjectsDbContext context, IdentityErrorDescriber? describer = null)
    : UserStore<NaturalPerson>(context, describer), IApplicationUserStore<NaturalPerson>
{
    public async Task<NaturalPerson?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await Context.Set<NaturalPerson>().SingleOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);
    }

    public Task<NaturalPerson?> GetOriginalAsync(NaturalPerson person, CancellationToken cancellationToken)
    {
        //使用跟踪器获取未更改的实体。
        return Task.FromResult(Context.Entry(person).OriginalValues.ToObject() as NaturalPerson);
    }
}