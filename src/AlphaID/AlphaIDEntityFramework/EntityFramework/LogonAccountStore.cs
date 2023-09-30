using DirectoryLogon;

namespace AlphaIDEntityFramework.EntityFramework;

public class LogonAccountStore : ILogonAccountStore
{
    private readonly DirectoryLogonDbContext dbContext;

    public LogonAccountStore(DirectoryLogonDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<LogonAccount> Accounts => this.dbContext.LogonAccounts;

    public async Task CreateAsync(LogonAccount account)
    {
        this.dbContext.LogonAccounts.Add(account);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(LogonAccount account)
    {
        this.dbContext.LogonAccounts.Remove(account);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(LogonAccount account)
    {
        this.dbContext.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
    }
}
