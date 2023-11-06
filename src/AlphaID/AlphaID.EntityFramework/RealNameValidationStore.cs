using IDSubjects;
using IDSubjects.RealName;

namespace AlphaID.EntityFramework;
public class RealNameValidationStore : IChineseIdCardValidationStore
{
    private readonly IdSubjectsDbContext dbContext;

    public RealNameValidationStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<ChineseIdCardValidation> RealNameValidations => this.dbContext.RealNameValidations;

    public async Task CreateAsync(ChineseIdCardValidation request)
    {
        this.dbContext.RealNameValidations.Add(request);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ChineseIdCardValidation request)
    {
        this.dbContext.RealNameValidations.Remove(request);
        await this.dbContext.SaveChangesAsync();
    }

    public ValueTask<ChineseIdCardValidation?> FindByIdAsync(int id)
    {
        return this.dbContext.RealNameValidations.FindAsync(id);
    }

    public Task<ChineseIdCardValidation?> GetCurrentAsync(NaturalPerson person)
    {
        var result = this.dbContext.RealNameValidations.Where(p => p.PersonId == person.Id && p.Result!.Accepted).OrderByDescending(p => p.Result!.ValidateTime).FirstOrDefault();
        return Task.FromResult(result);
    }

    public Task<ChineseIdCardValidation?> GetPendingRequestAsync(NaturalPerson person)
    {
        var result = this.dbContext.RealNameValidations.Where(p => p.PersonId == person.Id && p.Result == null).OrderByDescending(p => p.CommitTime).FirstOrDefault();
        return Task.FromResult(result);
    }

    public async Task UpdateAsync(ChineseIdCardValidation request)
    {
        this.dbContext.Entry(request).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await this.dbContext.SaveChangesAsync();
    }
}
