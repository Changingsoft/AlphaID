using DirectoryLogon;

namespace AlphaID.DirectoryLogon.EntityFramework;

public class QueryableLogonAccountStore : IQueryableLogonAccountStore
{
    private readonly DirectoryLogonDbContext dbContext;

    public QueryableLogonAccountStore(DirectoryLogonDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<LogonAccount> LogonAccounts => this.dbContext.LogonAccounts;

    public async Task<LogonAccount?> FindByLogonIdAsync(string logonId)
    {
        return await this.dbContext.LogonAccounts.FindAsync(logonId);
    }
}
