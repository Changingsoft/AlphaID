using IdSubjects.DirectoryLogon;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.DirectoryLogon.EntityFramework;

public class DirectoryAccountStore(DirectoryLogonDbContext dbContext) : IDirectoryAccountStore
{
    public IQueryable<DirectoryAccount> Accounts => dbContext.LogonAccounts.Include(p=> p.DirectoryServiceDescriptor);

    public async Task CreateAsync(DirectoryAccount account)
    {
        dbContext.LogonAccounts.Add(account);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(DirectoryAccount account)
    {
        dbContext.LogonAccounts.Remove(account);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(DirectoryAccount account)
    {
        dbContext.LogonAccounts.Update(account);
        await dbContext.SaveChangesAsync();
    }
}
