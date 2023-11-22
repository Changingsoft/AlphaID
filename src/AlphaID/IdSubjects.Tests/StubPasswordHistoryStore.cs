using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Tests;
public class StubPasswordHistoryStore : IPasswordHistoryStore
{
    private readonly HashSet<PasswordHistory> set = new HashSet<PasswordHistory>();

    public Task<IdentityResult> CreateAsync(PasswordHistory history)
    {
        this.set.Add(history);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(PasswordHistory history)
    {
        this.set.Remove(history);
        return Task.FromResult(IdentityResult.Success);
    }

    public IEnumerable<PasswordHistory> GetPasswords(NaturalPerson person, int historyLength)
    {
        return this.set.Where(h => h.UserId == person.Id).OrderByDescending(h => h.WhenCreated).Take(historyLength);
    }

    public Task TrimHistory(NaturalPerson person, int optionsRememberPasswordHistory)
    {
        //it is not important for test. just complete the task.
        return Task.CompletedTask;
    }

    public Task ClearAsync(NaturalPerson person)
    {
        this.set.Clear();
        return Task.CompletedTask;
    }
}
