using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.IdSubjects;

internal class PasswordHistoryStore(IdSubjectsDbContext dbContext) : IPasswordHistoryStore
{
    public async Task<IdentityResult> AddAsync(string data, string userId, DateTimeOffset timeOffset)
    {
        var history = new PasswordHistory { Data = data, UserId = userId, WhenCreated = timeOffset };
        dbContext.PasswordHistorySet.Add(history);
        await dbContext.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public IEnumerable<string> GetPasswords(string person, int historyLength)
    {
        IOrderedQueryable<PasswordHistory> resultSet = from history in dbContext.PasswordHistorySet
                                                       where history.UserId == person
                                                       orderby history.WhenCreated descending
                                                       select history;
        return resultSet.Take(historyLength).Select(his => his.Data);
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