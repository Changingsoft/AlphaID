using IdSubjects.DirectoryLogon;

namespace AlphaId.DirectoryLogon.EntityFramework;

public class DirectoryServiceStore : IDirectoryServiceStore
{
    private readonly DirectoryLogonDbContext dbContext;

    public DirectoryServiceStore(DirectoryLogonDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<DirectoryService> Services => this.dbContext.DirectoryServices;

    public async Task CreateAsync(DirectoryService service)
    {
        this.dbContext.DirectoryServices.Add(service);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(DirectoryService service)
    {
        this.dbContext.DirectoryServices.Remove(service);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<DirectoryService?> FindByIdAsync(int id)
    {
        return await this.dbContext.DirectoryServices.FindAsync(id);
    }

    public async Task UpdateAsync(DirectoryService service)
    {
        this.dbContext.Entry(service).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
    }
}
