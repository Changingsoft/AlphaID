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

    public IEnumerable<PasswordHistory> GetPasswords(string person, int historyLength)
    {
        var resultSet = from history in dbContext.PasswordHistorySet
                        where history.UserId == person
                        orderby history.WhenCreated descending
                        select history;
        return resultSet.Take(historyLength);
    }

    public async Task TrimHistory(string person, int optionsRememberPasswordHistory)
    {
        int anchor = await dbContext.PasswordHistorySet
            .Where(p => p.UserId == person)
            .OrderByDescending(p => p.WhenCreated)
            .Skip(optionsRememberPasswordHistory)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();
        await dbContext.PasswordHistorySet.Where(p => p.UserId == person && p.Id < anchor).ExecuteDeleteAsync();
    }

    public async Task ClearAsync(string person)
    {
        await dbContext.PasswordHistorySet.Where(p => p.UserId == person).ExecuteDeleteAsync();
    }
}
