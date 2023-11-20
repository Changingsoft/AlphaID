using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework;
internal class PasswordHistoryStore : IPasswordHistoryStore
{
    private readonly IdSubjectsDbContext dbContext;

    public PasswordHistoryStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IdentityResult> CreateAsync(PasswordHistory history)
    {
        this.dbContext.PasswordHistorySet.Add(history);
        await this.dbContext.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(PasswordHistory history)
    {
        this.dbContext.PasswordHistorySet.Remove(history);
        await this.dbContext.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public IEnumerable<PasswordHistory> GetPasswords(NaturalPerson person, int historyLength)
    {
        var resultSet = from history in this.dbContext.PasswordHistorySet
                        where history.UserId == person.Id
                        orderby history.WhenCreated descending
                        select history;
        return resultSet.Take(historyLength);
    }

    public async Task TrimHistory(NaturalPerson person, int optionsRememberPasswordHistory)
    {
        var anchor = await this.dbContext.PasswordHistorySet
            .Where(p => p.UserId == person.Id)
            .OrderByDescending(p => p.WhenCreated)
            .Skip(optionsRememberPasswordHistory)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();
        await this.dbContext.PasswordHistorySet.Where(p => p.UserId == person.Id && p.Id < anchor).ExecuteDeleteAsync();
    }

    public async Task ClearAsync(NaturalPerson person)
    {
        await this.dbContext.PasswordHistorySet.Where(p => p.UserId == person.Id).ExecuteDeleteAsync();
    }
}
