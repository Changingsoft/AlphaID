using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
/// 一个关于ApplicationUser的组合接口以便于实现ApplicationUserStore.
/// </summary>
public interface IApplicationUserStore<T> :
    IUserStore<T>
where T : ApplicationUser
{
    /// <summary>
    /// 通过手机号码查找自然人。
    /// </summary>
    /// <param name="phoneNumber">E.164 格式的手机号码。</param>
    /// <param name="cancellationToken">可取消令牌。</param>
    /// <returns></returns>
    Task<T?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
}