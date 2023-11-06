
namespace IDSubjects.DirectoryLogon.Tests;
internal class StubDirectoryServiceStore : IDirectoryServiceStore
{
    public IQueryable<DirectoryService> Services => throw new NotImplementedException();

    public Task CreateAsync(DirectoryService serivce)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(DirectoryService serivce)
    {
        throw new NotImplementedException();
    }

    public Task<DirectoryService?> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(DirectoryService serivce)
    {
        throw new NotImplementedException();
    }
}
