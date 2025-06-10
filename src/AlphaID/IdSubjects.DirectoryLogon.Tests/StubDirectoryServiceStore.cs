namespace IdSubjects.DirectoryLogon.Tests;

internal class StubDirectoryServiceStore : IDirectoryServiceStore
{
    public IQueryable<DirectoryService> Services => throw new NotImplementedException();

    public Task CreateAsync(DirectoryService service)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(DirectoryService service)
    {
        throw new NotImplementedException();
    }

    public Task<DirectoryService?> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(DirectoryService service)
    {
        throw new NotImplementedException();
    }
}