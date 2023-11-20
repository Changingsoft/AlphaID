using IdSubjects.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
public class PasswordHistoryManager
{
    private readonly IPasswordHistoryStore store;
    private readonly IPasswordHasher<NaturalPerson> passwordHasher;
    private readonly IdSubjectsPasswordOptions options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    /// <param name="passwordHasher"></param>
    /// <param name="options"></param>
    public PasswordHistoryManager(IPasswordHistoryStore store, IPasswordHasher<NaturalPerson> passwordHasher, IOptions<IdSubjectsOptions> options)
    {
        this.store = store;
        this.passwordHasher = passwordHasher;
        this.options = options.Value.Password;
    }

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
        var passwords = this.store.GetPasswords(person, this.options.RememberPasswordHistory);
        return passwords
            .Select(passHis => this.passwordHasher.VerifyHashedPassword(person, passHis.Data, password))
            .Any(result => result.HasFlag(PasswordVerificationResult.Success));
    }

    /// <summary>
    /// 将密码计入历史。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="password"></param>
    public async Task Pass(NaturalPerson person, string password)
    {
        await this.store.CreateAsync(new PasswordHistory()
        {
            Data = this.passwordHasher.HashPassword(person, password),
            UserId = person.Id,
            WhenCreated = this.TimeProvider.GetUtcNow(),
        });
        await this.store.TrimHistory(person, this.options.RememberPasswordHistory);
    }

    /// <summary>
    /// 清除用户的密码历史。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public async Task Clear(NaturalPerson person)
    {
        await this.store.ClearAsync(person);
    }
}
