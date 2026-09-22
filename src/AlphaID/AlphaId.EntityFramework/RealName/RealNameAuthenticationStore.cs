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
        // EF Core 9 同时在 EntityFrameworkQueryableExtensions 和 RelationalQueryableExtensions 中
        // 提供了 ExecuteDeleteAsync，直接调用会报 CS0121 歧义（EF Core 10 已移除后者，故仅 net8.0 目标报错）。
        // 显式限定为 EntityFrameworkQueryableExtensions，使 net8.0(EF9) 与 net10.0(EF10) 都能编译。
        IQueryable<RealNameAuthentication> query =
            dbContext.RealNameAuthentications.Where(a => a.PersonId == personId);
        await EntityFrameworkQueryableExtensions.ExecuteDeleteAsync(query);
        return IdOperationResult.Success;
    }

    public IQueryable<RealNameAuthentication> FindByPerson(ApplicationUser person)
    {
        return dbContext.RealNameAuthentications.Where(a => a.PersonId == person.Id);
    }
}