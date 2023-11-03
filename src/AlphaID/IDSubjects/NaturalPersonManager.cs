using IDSubjects.DependencyInjection;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Formats;
using System.Transactions;
using TimeZoneConverter;

namespace IDSubjects;

/// <summary>
/// 自然人管理器。
/// </summary>
public class NaturalPersonManager : UserManager<NaturalPerson>
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    /// <param name="optionsAccessor"></param>
    /// <param name="passwordHasher"></param>
    /// <param name="userValidators"></param>
    /// <param name="passwordValidators"></param>
    /// <param name="keyNormalizer"></param>
    /// <param name="errors"></param>
    /// <param name="services"></param>
    /// <param name="logger"></param>
    public NaturalPersonManager(IUserStore<NaturalPerson> store,
                                IOptions<IDSubjectsOptions> optionsAccessor,
                                IPasswordHasher<NaturalPerson> passwordHasher,
                                IEnumerable<IUserValidator<NaturalPerson>> userValidators,
                                IEnumerable<IPasswordValidator<NaturalPerson>> passwordValidators,
                                ILookupNormalizer keyNormalizer,
                                NaturalPersonIdentityErrorDescriber errors,
                                IServiceProvider services,
                                ILogger<NaturalPersonManager> logger)
        : base(store,
               optionsAccessor,
               passwordHasher,
               userValidators,
               passwordValidators,
               keyNormalizer,
               errors,
               services,
               logger)
    {
    }

    /// <summary>
    /// Find person via mobile.
    /// </summary>
    /// <param name="mobile"></param>
    /// <returns></returns>
    public virtual Task<NaturalPerson?> FindByMobileAsync(string mobile)
    {
        if (!MobilePhoneNumber.TryParse(mobile, out var phoneNumber))
            return Task.FromResult(default(NaturalPerson));
        var phoneNumberString = phoneNumber.ToString();
        var person = this.Users.SingleOrDefault(p => p.PhoneNumber == phoneNumberString);
        return Task.FromResult(person);
    }

    /// <summary>
    /// 当身份验证成功时，调用此方法以记录包括登录次数、上次登录时间、登录方式等信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="authenticationMethod"></param>
    /// <returns></returns>
    public virtual Task AccessSuccededAsync(NaturalPerson person, string authenticationMethod)
    {
        //todo 记录任何登录成功次数、上次登录时间，登录方式，登录IP等。
        this.Logger.LogInformation("用户{person}成功执行了登录，登录成功计数器+1，记录登录时间{time}，登录方式为：{authenticationMethod}", person, DateTimeOffset.UtcNow, authenticationMethod);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    public NaturalPersonIdentityErrorDescriber NaturalPersonIdentityErrorDescriber => this.ErrorDescriber as NaturalPersonIdentityErrorDescriber ?? throw new InvalidCastException();

    /// <summary>
    /// 更改自然人的名称信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="chinesePersonName"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ChangeNameAsync(NaturalPerson person, ChinesePersonName chinesePersonName)
    {
        person.SetName(chinesePersonName);
        await this.UpdateAsync(person);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 强制更改用户的姓名信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="chinesePersonName"></param>
    /// <returns></returns>
    public async Task ForceChangeNameAsync(NaturalPerson person, ChinesePersonName chinesePersonName)
    {
        person.SetName(chinesePersonName);
        await this.UpdateAsync(person);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> CreateAsync(NaturalPerson user, string password)
    {
        user.PasswordLastSet = DateTimeOffset.UtcNow;
        return await base.CreateAsync(user, password);
    }

    /// <summary>
    /// 已重写。创建用户。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> CreateAsync(NaturalPerson user)
    {
        var utcNow = DateTimeOffset.UtcNow;
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        return await base.CreateAsync(user);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected override async Task<IdentityResult> UpdateUserAsync(NaturalPerson user)
    {
        user.WhenChanged = DateTimeOffset.UtcNow;
        return await base.UpdateUserAsync(user);
    }

    /// <summary>
    /// 尝试设置时区。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tzName"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> SetTimeZone(NaturalPerson user, string tzName)
    {
        if (TZConvert.KnownIanaTimeZoneNames.Any(p => p == tzName))
        {
            user.TimeZone = tzName;
            return Task.FromResult(IdentityResult.Success);
        }
        return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "Invalid_TzInfo", Description = "Invalid time zone name." }));
    }

    /// <summary>
    /// 已重写。移除本地登录密码。
    /// 该方法还会清空<see cref="NaturalPerson.PasswordLastSet"/>的值。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> RemovePasswordAsync(NaturalPerson user)
    {
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        IdentityResult result = await base.RemovePasswordAsync(user);
        if (!result.Succeeded)
            return result;
        user.PasswordLastSet = null;
        result = await this.UpdateAsync(user);
        if (!result.Succeeded)
        {
            this.Logger.LogError("用户密码已删除，但设置PasswordLastSet时出错，事务已回滚。", result);
            return result;
        }
        return result;
    }

    /// <summary>
    /// 已重写。修改密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="currentPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ChangePasswordAsync(NaturalPerson user, string currentPassword, string newPassword)
    {
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        IdentityResult result = await base.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return result;
        user.PasswordLastSet = DateTimeOffset.UtcNow;
        result = await this.UpdateAsync(user);
        if (!result.Succeeded)
        {
            this.Logger.LogError("用户密码已更新，但设置PasswordLastSet时出错，事务已回滚。", result);
            return result;
        }
        trans.Complete();
        return result;
    }

    /// <summary>
    /// 已重写。用户重设密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ResetPasswordAsync(NaturalPerson user, string token, string newPassword)
    {
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        IdentityResult result = await base.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            return result;
        //todo 需要从选项确定是否要清除PasswordLastSet
        if (true)
        {
            user.PasswordLastSet = null;
            result = await this.UpdateAsync(user);
            this.Logger.LogInformation("已将{user}的PasswordLastSet清除", user);
            if (!result.Succeeded)
            {
                this.Logger.LogError("清除PasswordLastSet时出错", result);
                return result;
            }
        }
        trans.Complete();
        return IdentityResult.Success;
    }

    /// <summary>
    /// 已重写，更新密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newPassword"></param>
    /// <param name="validatePassword"></param>
    /// <returns></returns>
    protected override async Task<IdentityResult> UpdatePasswordHash(NaturalPerson user, string newPassword, bool validatePassword)
    {
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        IdentityResult result = await base.UpdatePasswordHash(user, newPassword, validatePassword);
        if (!result.Succeeded)
            return result;
        user.PasswordLastSet = DateTimeOffset.UtcNow;
        result = await this.UpdateAsync(user);
        if (!result.Succeeded)
        {
            this.Logger.LogError("用户密码已更新，但设置PasswordLastSet时出错，事务已回滚。", result);
            return result;
        }
        trans.Complete();
        return IdentityResult.Success;
    }

    /// <summary>
    /// 管理员重置用户密码。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="newPassword"></param>
    /// <param name="mustChangePassword"></param>
    /// <param name="unlockUser"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> AdminResetPasswordAsync(NaturalPerson person, string newPassword, bool mustChangePassword, bool unlockUser)
    {
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        IdentityResult result = await this.UpdatePasswordHash(person, newPassword, true);
        if (!result.Succeeded)
            return result;
        if (mustChangePassword)
        {
            person.PasswordLastSet = null;
            result = await this.UpdateAsync(person);
            this.Logger.LogInformation("已将{user}的PasswordLastSet清除", person);
            if (!result.Succeeded)
            {
                this.Logger.LogError("用户密码已设置，但清除PasswordLastSet时出错，事务已回滚。", result);
                return result;
            }
        }
        if (unlockUser)
            result = await this.UnlockPersonAsync(person);
        if (!result.Succeeded)
        {
            this.Logger.LogError("用户密码已设置，但设置解锁时出错，事务已回滚。", result);
            return result;
        }
        trans.Complete();
        return IdentityResult.Success;
    }

    /// <summary>
    /// 解锁用户。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> UnlockPersonAsync(NaturalPerson person)
    {
        this.Logger.LogDebug("正在解锁用户{user}", person);
        if (await this.IsLockedOutAsync(person))
        {
            return await this.SetLockoutEndDateAsync(person, null);
        }
        this.Logger.LogDebug("用户{user}未锁定，此操作无效果。", person);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 已重写，添加密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> AddPasswordAsync(NaturalPerson user, string password)
    {
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        IdentityResult result = await base.AddPasswordAsync(user, password);
        if (!result.Succeeded)
            return result;
        user.PasswordLastSet = DateTimeOffset.UtcNow;
        result = await this.UpdateAsync(user);
        if (!result.Succeeded)
        {
            this.Logger.LogError("添加密码时设置PasswordLastSet属性出错。", result);
            return result;
        }
        trans.Complete();
        return IdentityResult.Success;
    }

    /// <summary>
    /// 设置头像。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="contentType"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public async Task<IdentityResult> SetProfilePictureAsync(NaturalPerson person, string contentType, byte[] bytes)
    {
        try
        {
            var info = Image.Identify(bytes);
        }
        catch (InvalidImageContentException ex)
        {
            this.Logger?.LogWarning(ex, "传入的数据不是有效的图片内容。");
            throw;
        }

        person.Avatar = new BinaryDataInfo()
        {
            Data = bytes,
            MimeType = contentType,
        };
        var result = await this.UpdateUserAsync(person);
        return result;
    }

    /// <summary>
    /// 清除用户的头像。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public async Task<IdentityResult> ClearProfilePictureAsync(NaturalPerson person)
    {
        person.Avatar = null;
        var result = await this.UpdateUserAsync(person);
        if (result.Succeeded)
            this.Logger?.LogInformation("用户头像已清除。");
        else
            this.Logger?.LogWarning("清除用户头像时不成功。", result);
        return result;
    }

    public override async Task<IdentityResult> SetPhoneNumberAsync(NaturalPerson user, string? phoneNumber)
    {
        var result = await base.SetPhoneNumberAsync(user, phoneNumber);
        if(result.Succeeded)
        {
            //todo 考虑从选项来控制是否自动将PhoneNumberConfirmed设置为true
            user.PhoneNumberConfirmed = true;
            return await this.UpdateAsync(user);
        }
        return result;
    }
}
