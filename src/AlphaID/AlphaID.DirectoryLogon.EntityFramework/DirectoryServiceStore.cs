using IDSubjects.DirectoryLogon;

namespace AlphaID.DirectoryLogon.EntityFramework;

public class DirectoryServiceStore : IDirectoryServiceStore
{
    private readonly DirectoryLogonDbContext dbContext;

    public DirectoryServiceStore(DirectoryLogonDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<DirectoryService> Services => this.dbContext.DirectoryServices;

    public async Task CreateAsync(DirectoryService serivce)
    {
        this.dbContext.DirectoryServices.Add(serivce);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(DirectoryService serivce)
    {
        this.dbContext.DirectoryServices.Remove(serivce);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<DirectoryService?> FindByIdAsync(int id)
    {
        return await this.dbContext.DirectoryServices.FindAsync(id);
    }

    public async Task UpdateAsync(DirectoryService serivce)
    {
        this.dbContext.Entry(serivce).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await this.dbContext.SaveChangesAsync();
    }
}
