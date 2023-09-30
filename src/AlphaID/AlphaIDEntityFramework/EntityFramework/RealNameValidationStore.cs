using IDSubjects;
using IDSubjects.RealName;

namespace AlphaIDEntityFramework.EntityFramework;
public class RealNameValidationStore : IChineseIDCardValidationStore
{
    private readonly IDSubjectsDbContext dbContext;

    public RealNameValidationStore(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<ChineseIDCardValidation> RealNameValidations => this.dbContext.RealNameValidations;

    public async Task CreateAsync(ChineseIDCardValidation request)
    {
        this.dbContext.RealNameValidations.Add(request);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ChineseIDCardValidation request)
    {
        this.dbContext.RealNameValidations.Remove(request);
        await this.dbContext.SaveChangesAsync();
    }

    public ValueTask<ChineseIDCardValidation?> FindByIdAsync(int id)
    {
        return this.dbContext.RealNameValidations.FindAsync(id);
    }

    public Task<ChineseIDCardValidation?> GetCurrentAsync(NaturalPerson person)
    {
        var result = this.dbContext.RealNameValidations.Where(p => p.PersonId == person.Id && p.Result!.Accepted).OrderByDescending(p => p.Result!.ValidateTime).FirstOrDefault();
        return Task.FromResult(result);
    }

    public Task<ChineseIDCardValidation?> GetPendingRequestAsync(NaturalPerson person)
    {
        var result = this.dbContext.RealNameValidations.Where(p => p.PersonId == person.Id && p.Result == null).OrderByDescending(p => p.CommitTime).FirstOrDefault();
        return Task.FromResult(result);
    }

    public async Task UpdateAsync(ChineseIDCardValidation request)
    {
        this.dbContext.Entry(request).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await this.dbContext.SaveChangesAsync();
    }
}
