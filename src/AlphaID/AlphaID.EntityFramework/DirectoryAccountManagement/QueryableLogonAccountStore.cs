using IdSubjects.DirectoryLogon;

namespace AlphaId.EntityFramework.DirectoryAccountManagement;

public class QueryableLogonAccountStore(DirectoryLogonDbContext dbContext) : IQueryableLogonAccountStore
{
    public IQueryable<DirectoryAccount> LogonAccounts => dbContext.LogonAccounts;

    public async Task<DirectoryAccount?> FindByLogonIdAsync(string logonId)
    {
        return await dbContext.LogonAccounts.FindAsync(logonId);
    }
}