namespace IdSubjects.RealName.Tests;

internal class StubRealNameAuthenticationStore : IRealNameAuthenticationStore
{
    private readonly HashSet<RealNameAuthentication> _set = [];

    public IQueryable<RealNameAuthentication> Authentications => _set.AsQueryable();

    public Task<IdOperationResult> CreateAsync(RealNameAuthentication authentication)
    {
        _set.Add(authentication);
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> UpdateAsync(RealNameAuthentication authentication)
    {
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> DeleteAsync(RealNameAuthentication authentication)
    {
        _set.Remove(authentication);
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> DeleteByPersonIdAsync(string personId)
    {
        _set.RemoveWhere(a => a.PersonId == personId);
        return Task.FromResult(IdOperationResult.Success);
    }

    public IQueryable<RealNameAuthentication> FindByPerson(NaturalPerson person)
    {
        return _set.Where(a => a.PersonId == person.Id).AsQueryable();
    }

    public Task<RealNameAuthentication?> FindByIdAsync(string id)
    {
        return Task.FromResult(_set.FirstOrDefault(a => a.Id == id));
    }
}