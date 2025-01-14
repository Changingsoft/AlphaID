using IdSubjects.DirectoryLogon;

namespace AlphaId.EntityFramework.DirectoryAccountManagement;

public class DirectoryServiceDescriptorStore(DirectoryLogonDbContext dbContext) : IDirectoryServiceDescriptorStore
{
    public IQueryable<DirectoryServiceDescriptor> Services => dbContext.DirectoryServices;

    public async Task CreateAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        dbContext.DirectoryServices.Add(serviceDescriptor);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        dbContext.DirectoryServices.Remove(serviceDescriptor);
        await dbContext.SaveChangesAsync();
    }

    public async Task<DirectoryServiceDescriptor?> FindByIdAsync(int id)
    {
        return await dbContext.DirectoryServices.FindAsync(id);
    }

    public async Task UpdateAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        dbContext.DirectoryServices.Update(serviceDescriptor);
        await dbContext.SaveChangesAsync();
    }
}