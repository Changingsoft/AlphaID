using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
/// 提供密码历史记录存取的接口。
/// </summary>
public interface IPasswordHistoryStore
{
    /// <summary>
    /// 创建密码历史记录。
    /// </summary>
    /// <param name="history"></param>
    /// <returns></returns>
    Task<IdentityResult> CreateAsync(PasswordHistory history);

    /// <summary>
    /// 删除密码历史记录
    /// </summary>
    /// <param name="history"></param>
    /// <returns></returns>
    Task<IdentityResult> DeleteAsync(PasswordHistory history);

    /// <summary>
    /// 获取指定自然人的密码历史。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="historyLength"></param>
    /// <returns></returns>
    IEnumerable<PasswordHistory> GetPasswords(NaturalPerson person, int historyLength);

    /// <summary>
    /// 裁剪指定自然人的密码历史。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="optionsRememberPasswordHistory"></param>
    Task TrimHistory(NaturalPerson person, int optionsRememberPasswordHistory);

    /// <summary>
    /// 清除指定自然人的所有密码历史。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task ClearAsync(NaturalPerson person);
}
