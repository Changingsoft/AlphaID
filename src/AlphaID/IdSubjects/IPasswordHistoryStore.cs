using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
public interface IPasswordHistoryStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="history"></param>
    /// <returns></returns>
    Task<IdentityResult> CreateAsync(PasswordHistory history);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="history"></param>
    /// <returns></returns>
    Task<IdentityResult> DeleteAsync(PasswordHistory history);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="historyLength"></param>
    /// <returns></returns>
    IEnumerable<PasswordHistory> GetPasswords(NaturalPerson person, int historyLength);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="optionsRememberPasswordHistory"></param>
    Task TrimHistory(NaturalPerson person, int optionsRememberPasswordHistory);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task ClearAsync(NaturalPerson person);
}
