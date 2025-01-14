using IdSubjects;
using IdSubjects.RealName;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.RealName;

internal class RealNameAuthenticationStore(RealNameDbContext dbContext) : IRealNameAuthenticationStore
{
    public IQueryable<RealNameAuthentication> Authentications => dbContext.RealNameAuthentications.AsNoTracking();

    public async Task<RealNameAuthentication?> FindByIdAsync(string id)
    {
        return await dbContext.RealNameAuthentications.FindAsync(id);
    }

    public async Task<IdOperationResult> CreateAsync(RealNameAuthentication realNameState)
    {
        dbContext.RealNameAuthentications.Add(realNameState);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(RealNameAuthentication realNameState)
    {
        dbContext.RealNameAuthentications.Update(realNameState);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(RealNameAuthentication realNameState)
    {
        dbContext.RealNameAuthentications.Remove(realNameState);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteByPersonIdAsync(string personId)
    {
        await dbContext.RealNameAuthentications.Where(a => a.PersonId == personId).ExecuteDeleteAsync();
        return IdOperationResult.Success;
    }

    public IQueryable<RealNameAuthentication> FindByPerson(NaturalPerson person)
    {
        return dbContext.RealNameAuthentications.Where(a => a.PersonId == person.Id);
    }
}