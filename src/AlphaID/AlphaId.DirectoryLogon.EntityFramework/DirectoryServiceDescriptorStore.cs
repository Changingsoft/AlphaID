using IdSubjects.DirectoryLogon;

namespace AlphaId.DirectoryLogon.EntityFramework;

public class DirectoryServiceDescriptorStore(DirectoryLogonDbContext dbContext) : IDirectoryServiceDescriptorStore
{
    public IQueryable<DirectoryServiceDescriptor> Services => dbContext.DirectoryServices;

    public async Task CreateAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        dbContext.DirectoryServices.Add(serviceDescriptor);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        dbContext.DirectoryServices.Remove(serviceDescriptor);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<DirectoryServiceDescriptor?> FindByIdAsync(int id)
    {
        return await dbContext.DirectoryServices.FindAsync(id);
    }

    public async Task UpdateAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        dbContext.Entry(serviceDescriptor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _ = await dbContext.SaveChangesAsync();
    }
}
