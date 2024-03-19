using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework;
internal class PasswordHistoryStore(IdSubjectsDbContext dbContext) : IPasswordHistoryStore
{
    public async Task<IdentityResult> CreateAsync(PasswordHistory history)
    {
        dbContext.PasswordHistorySet.Add(history);
        await dbContext.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(PasswordHistory history)
    {
        dbContext.PasswordHistorySet.Remove(history);
        await dbContext.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public IEnumerable<PasswordHistory> GetPasswords(NaturalPerson person, int historyLength)
    {
        var resultSet = from history in dbContext.PasswordHistorySet
                        where history.UserId == person.Id
                        orderby history.WhenCreated descending
                        select history;
        return resultSet.Take(historyLength);
    }

    public async Task TrimHistory(NaturalPerson person, int optionsRememberPasswordHistory)
    {
        var anchor = await dbContext.PasswordHistorySet
            .Where(p => p.UserId == person.Id)
            .OrderByDescending(p => p.WhenCreated)
            .Skip(optionsRememberPasswordHistory)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();
        await dbContext.PasswordHistorySet.Where(p => p.UserId == person.Id && p.Id < anchor).ExecuteDeleteAsync();
    }

    public async Task ClearAsync(NaturalPerson person)
    {
        await dbContext.PasswordHistorySet.Where(p => p.UserId == person.Id).ExecuteDeleteAsync();
    }
}
