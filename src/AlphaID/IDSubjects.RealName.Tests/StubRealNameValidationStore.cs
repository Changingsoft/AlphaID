using IDSubjects;
using IDSubjects.RealName;

namespace IDSubjectsTests.RealName;
internal class StubRealNameValidationStore : IChineseIDCardValidationStore
{
    private readonly HashSet<ChineseIDCardValidation> set = new();

    public IQueryable<ChineseIDCardValidation> RealNameValidations => this.set.AsQueryable();

    public Task CreateAsync(ChineseIDCardValidation request)
    {
        var lastId = this.set.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
        var nextId = lastId + 1;
        request.Id = nextId;
        this.set.Add(request);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ChineseIDCardValidation request)
    {
        this.set.Remove(request);
        return Task.CompletedTask;
    }

    public ValueTask<ChineseIDCardValidation?> FindByIdAsync(int id)
    {
        var result = this.set.FirstOrDefault(x => x.Id == id);
        return ValueTask.FromResult(result);
    }

    public Task<ChineseIDCardValidation?> GetCurrentAsync(NaturalPerson person)
    {
        var result = this.set.OrderByDescending(p => p.Result!.ValidateTime).FirstOrDefault(p => p.Result!.Accepted && p.PersonId == person.Id);
        return Task.FromResult(result);
    }

    public Task<ChineseIDCardValidation?> GetPendingRequestAsync(NaturalPerson person)
    {
        var result = this.set.OrderByDescending(p => p.CommitTime).FirstOrDefault(p => p.Result == null && p.PersonId == person.Id);
        return Task.FromResult(result);
    }

    public Task UpdateAsync(ChineseIDCardValidation request)
    {
        //do nothing here.
        return Task.CompletedTask;
    }
}
