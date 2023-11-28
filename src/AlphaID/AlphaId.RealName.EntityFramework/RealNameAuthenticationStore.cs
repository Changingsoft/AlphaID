using IdSubjects;
using IdSubjects.RealName;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.RealName.EntityFramework;
internal class RealNameAuthenticationStore : IRealNameAuthenticationStore
{
    private readonly RealNameDbContext dbContext;

    public RealNameAuthenticationStore(RealNameDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<RealNameAuthentication> Authentications => this.dbContext.RealNameAuthentications.AsNoTracking();

    public async Task<RealNameAuthentication?> FindByIdAsync(string id)
    {
        return await this.dbContext.RealNameAuthentications.FindAsync(id);
    }

    public async Task<IdOperationResult> CreateAsync(RealNameAuthentication realNameState)
    {
        this.dbContext.RealNameAuthentications.Add(realNameState);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(RealNameAuthentication realNameState)
    {

        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(RealNameAuthentication realNameState)
    {
        this.dbContext.RealNameAuthentications.Remove(realNameState);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteByPersonIdAsync(string personId)
    {
        await this.dbContext.RealNameAuthentications.Where(a => a.PersonId == personId).ExecuteDeleteAsync();
        return IdOperationResult.Success;
    }

    public IQueryable<RealNameAuthentication> FindByPerson(NaturalPerson person)
    {
        return this.dbContext.RealNameAuthentications.Where(a => a.PersonId == person.Id);
    }
}
