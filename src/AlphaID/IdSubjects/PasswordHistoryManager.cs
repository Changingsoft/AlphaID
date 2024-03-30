using IdSubjects.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="store"></param>
/// <param name="passwordHasher"></param>
/// <param name="options"></param>
public class PasswordHistoryManager(IPasswordHistoryStore store, IPasswordHasher<NaturalPerson> passwordHasher, IOptions<IdSubjectsOptions> options)
{
    private readonly IdSubjectsPasswordOptions _options = options.Value.Password;

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 命中指定用户的密码历史。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="password"></param>
    /// <returns>如果命中，则返回true，否则返回false。</returns>
    public bool Hit(NaturalPerson person, string password)
    {
        //取出密码历史
        var passwords = store.GetPasswords(person, _options.RememberPasswordHistory);
        return passwords
            .Select(passHis => passwordHasher.VerifyHashedPassword(person, passHis.Data, password))
            .Any(result => result.HasFlag(PasswordVerificationResult.Success));
    }

    /// <summary>
    /// 将密码计入历史。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="password"></param>
    public async Task Pass(NaturalPerson person, string password)
    {
        await store.CreateAsync(new PasswordHistory()
        {
            Data = passwordHasher.HashPassword(person, password),
            UserId = person.Id,
            WhenCreated = TimeProvider.GetUtcNow(),
        });
        await store.TrimHistory(person, _options.RememberPasswordHistory);
    }

    /// <summary>
    /// 清除用户的密码历史。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public Task Clear(NaturalPerson person)
    {
        return store.ClearAsync(person);
    }
}
