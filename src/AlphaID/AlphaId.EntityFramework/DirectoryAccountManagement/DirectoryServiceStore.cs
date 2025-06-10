using IdSubjects.DirectoryLogon;

namespace AlphaId.EntityFramework.DirectoryAccountManagement;

public class DirectoryServiceStore(DirectoryLogonDbContext dbContext) : IDirectoryServiceStore
{
    public IQueryable<DirectoryService> Services => dbContext.DirectoryServices;

    public async Task CreateAsync(DirectoryService service)
    {
        dbContext.DirectoryServices.Add(service);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(DirectoryService service)
    {
        dbContext.DirectoryServices.Remove(service);
        await dbContext.SaveChangesAsync();
    }

    public async Task<DirectoryService?> FindByIdAsync(int id)
    {
        return await dbContext.DirectoryServices.FindAsync(id);
    }

    public async Task UpdateAsync(DirectoryService service)
    {
        dbContext.DirectoryServices.Update(service);
        await dbContext.SaveChangesAsync();
    }
}