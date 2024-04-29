using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Tests;
public class StubPasswordHistoryStore : IPasswordHistoryStore
{
    private readonly HashSet<PasswordHistory> _set = [];

    public Task<IdentityResult> CreateAsync(PasswordHistory history)
    {
        _set.Add(history);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(PasswordHistory history)
    {
        _set.Remove(history);
        return Task.FromResult(IdentityResult.Success);
    }

    public IEnumerable<PasswordHistory> GetPasswords(string person, int historyLength)
    {
        return _set.Where(h => h.UserId == person).OrderByDescending(h => h.WhenCreated).Take(historyLength);
    }

    public Task TrimHistory(string person, int optionsRememberPasswordHistory)
    {
        // It is not important for test. just complete the task.
        return Task.CompletedTask;
    }

    public Task ClearAsync(string person)
    {
        _set.Clear();
        return Task.CompletedTask;
    }
}
