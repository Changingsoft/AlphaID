
namespace IdSubjects.DirectoryLogon.Tests;
internal class StubDirectoryServiceDescriptorStore : IDirectoryServiceDescriptorStore
{
    public IQueryable<DirectoryServiceDescriptor> Services => throw new NotImplementedException();

    public Task CreateAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        throw new NotImplementedException();
    }

    public Task<DirectoryServiceDescriptor?> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(DirectoryServiceDescriptor serviceDescriptor)
    {
        throw new NotImplementedException();
    }
}
