using IdSubjects;
using IdSubjects.RealName;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.RealName.EntityFramework;
internal class RealNameStateStore : IRealNameStateStore
{
    private readonly RealNameDbContext dbContext;

    public RealNameStateStore(RealNameDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<RealNameState> RealNameStates => this.dbContext.RealNameStates.AsNoTracking();

    public async Task<RealNameState?> FindByIdAsync(string id)
    {
        return await this.dbContext.RealNameStates.FindAsync(id);
    }

    public async Task<IdOperationResult> CreateAsync(RealNameState realNameState)
    {
        this.dbContext.RealNameStates.Add(realNameState);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(RealNameState realNameState)
    {

        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> DeleteAsync(RealNameState realNameState)
    {
        this.dbContext.RealNameStates.Remove(realNameState);
        await this.dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}
