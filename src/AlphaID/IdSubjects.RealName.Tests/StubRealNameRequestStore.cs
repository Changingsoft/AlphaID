using IdSubjects.RealName.Requesting;

namespace IdSubjects.RealName.Tests;

internal class StubRealNameRequestStore : IRealNameRequestStore
{
    private readonly HashSet<RealNameRequest> _set = [];

    public Task<IdOperationResult> CreateAsync(RealNameRequest request)
    {
        int lastId = _set.OrderByDescending(r => r.Id).Select(r => r.Id).FirstOrDefault();
        request.Id = lastId + 1;
        _set.Add(request);
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> UpdateAsync(RealNameRequest request)
    {
        //do nothing.
        return Task.FromResult(IdOperationResult.Success);
    }

    public IQueryable<RealNameRequest> Requests => _set.AsQueryable();

    public Task<RealNameRequest?> FindByIdAsync(int id)
    {
        return Task.FromResult(_set.FirstOrDefault(r => r.Id == id));
    }
}