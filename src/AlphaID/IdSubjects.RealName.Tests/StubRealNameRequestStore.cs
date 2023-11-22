using IdSubjects.RealName.Requesting;

namespace IdSubjects.RealName.Tests;
internal class StubRealNameRequestStore : IRealNameRequestStore
{
    private readonly HashSet<RealNameRequest> set = new();

    public Task<IdOperationResult> CreateAsync(RealNameRequest request)
    {
        var lastId = this.set.OrderByDescending(r => r.Id).Select(r => r.Id).FirstOrDefault();
        request.Id = lastId + 1;
        this.set.Add(request);
        return Task.FromResult(IdOperationResult.Success);
    }

    public Task<IdOperationResult> UpdateAsync(RealNameRequest request)
    {
        //do nothing.
        return Task.FromResult(IdOperationResult.Success);
    }
}
