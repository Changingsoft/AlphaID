namespace IdSubjects.RealName.Tests;
internal class StubRealNameAuthenticationStore : IRealNameAuthenticationStore
{
    private readonly HashSet<RealNameAuthentication> set = [];

    public IQueryable<RealNameAuthentication> Authentications => this.set.AsQueryable();
    public Task<IdOperationResult> CreateAsync(RealNameAuthentication authentication)
    {
        this.set.Add(authentication);
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> UpdateAsync(RealNameAuthentication authentication)
    {
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> DeleteAsync(RealNameAuthentication authentication)
    {
        this.set.Remove(authentication);
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> DeleteByPersonIdAsync(string personId)
    {
        this.set.RemoveWhere(a => a.PersonId == personId);
        return Task.FromResult(IdOperationResult.Success);
    }

    public IQueryable<RealNameAuthentication> FindByPerson(NaturalPerson person)
    {
        return this.set.Where(a => a.PersonId == person.Id).AsQueryable();
    }

    public Task<RealNameAuthentication?> FindByIdAsync(string id)
    {
        return Task.FromResult(this.set.FirstOrDefault(a => a.Id == id));
    }
}
