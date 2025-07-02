using System.Transactions;
using IdSubjects.SecurityAuditing;
using IdSubjects.SecurityAuditing.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using TimeZoneConverter;

namespace IdSubjects;

/// <summary>
/// 自然人管理器。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="store">一个<see cref="IApplicationUserStore{T}"/>的实现，提供存取用户的能力。</param>
/// <param name="optionsAccessor">标识选项。</param>
/// <param name="passwordHasher">密码哈希计算器。</param>
/// <param name="userValidators">用户验证器集合。</param>
/// <param name="passwordValidators">密码验证器集合。</param>
/// <param name="keyNormalizer">键规范化处理器。</param>
/// <param name="errors">错误描述器。</param>
/// <param name="services">服务提供器。</param>
/// <param name="logger">日志记录器。</param>
/// <param name="passwordLifetimeOptions">密码生存期选项。</param>
/// <param name="eventService">事件服务。</param>
public class ApplicationUserManager<T>(
    IUserStore<T> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<T> passwordHasher,
    IEnumerable<IUserValidator<T>> userValidators,
    IEnumerable<IPasswordValidator<T>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<ApplicationUserManager<T>> logger,
    IOptions<PasswordLifetimeOptions> passwordLifetimeOptions,
    IEventService eventService)
    : UserManager<T>(store,
        optionsAccessor,
        passwordHasher,
        userValidators,
        passwordValidators,
        keyNormalizer,
        errors,
        services,
        logger)
where T : ApplicationUser
{
    /// <summary>
    /// 获取密码声明周期选项。
    /// </summary>
    public virtual PasswordLifetimeOptions PasswordLifetime => passwordLifetimeOptions.Value;

    /// <summary>
    /// 获取 IApplicationUserStore。该属性已替换原属性。
    /// </summary>
    public new IApplicationUserStore<T> Store { get; } = store as IApplicationUserStore<T> ?? throw new ArgumentNullException(nameof(store));

    /// <summary>
    /// 获取错误描述器。
    /// </summary>
    public ApplicationUserIdentityErrorDescriber AppErrorDescriber { get; } = errors as ApplicationUserIdentityErrorDescriber ?? throw new ArgumentNullException(nameof(errors));

    /// <summary>
    /// 获取或设置时间提供器以便于可测试性。
    /// </summary>
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 获取审计事件服务。
    /// </summary>
    protected IEventService EventService { get; } = eventService;

    /// <summary>
    /// 通过移动电话号码查找自然人。
    /// </summary>
    /// <param name="mobile">移动电话号码，支持不带国际区号的11位号码格式或标准 E.164 格式。</param>
    /// <returns>返回找到的自然人。如果没有找到，则返回null。</returns>
    public virtual async Task<T?> FindByMobileAsync(string mobile)
    {
        if (!MobilePhoneNumber.TryParse(mobile, out MobilePhoneNumber phoneNumber))
            return null;
        string phoneNumberString = phoneNumber.ToString();
        T? person = await Store.FindByPhoneNumberAsync(phoneNumberString, CancellationToken);
        return person;
    }

    /// <inheritdoc />
    public override async Task<IdentityResult> CreateAsync(T user)
    {
        DateTimeOffset utcNow = TimeProvider.GetUtcNow();
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        IdentityResult result = await base.CreateAsync(user);
        if (result.Succeeded)
            await EventService.RaiseAsync(new CreatePersonSuccessEvent(user.UserName));
        else
            await EventService.RaiseAsync(new CreatePersonFailureEvent(user.UserName, result.Errors));
        return result;
    }


    /// <inheritdoc />
    public override async Task<IdentityResult> CreateAsync(T user, string password)
    {
        DateTimeOffset utcNow = TimeProvider.GetUtcNow();
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        user.PasswordLastSet = utcNow;
        LogUsedPassword(user, password);
        IdentityResult result = await base.CreateAsync(user, password);
        if (result.Succeeded)
            await EventService.RaiseAsync(new CreatePersonSuccessEvent(user.UserName));
        else
            await EventService.RaiseAsync(new CreatePersonFailureEvent(user.UserName, result.Errors));

        return result;
    }

    /// <summary>
    /// 使用指定的密码创建用户。并且指示用户在登录时是否必须更改密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="mustChangePassword">如果为true，则用户登录时必须修改密码。如果为false，和<see cref="CreateAsync(T, string)"/>等效。</param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> CreateAsync(T user, string password, bool mustChangePassword)
    {
        DateTimeOffset utcNow = TimeProvider.GetUtcNow();
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        if (!mustChangePassword)
        {
            user.PasswordLastSet = utcNow;
        }
        LogUsedPassword(user, password);
        IdentityResult result = await base.CreateAsync(user, password);
        if (result.Succeeded)
            await EventService.RaiseAsync(new CreatePersonSuccessEvent(user.UserName));
        else
            await EventService.RaiseAsync(new CreatePersonFailureEvent(user.UserName, result.Errors));
        return result;
    }

    /// <summary>
    /// 当身份验证成功时，调用此方法以记录包括登录次数、上次登录时间、登录方式等信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="authenticationMethod"></param>
    /// <returns></returns>
    public virtual Task AccessSuccededAsync(T person, string authenticationMethod)
    {
        //todo 记录任何登录成功次数、上次登录时间，登录方式，登录IP等。
        Logger.LogInformation("用户{person}成功执行了登录，登录成功计数器+1，记录登录时间{time}，登录方式为：{authenticationMethod}", person,
            TimeProvider.GetUtcNow(), authenticationMethod);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 已重写。若用户实现不对账户相关字段变更时，不应在用户实现中调用该方法。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected override Task<IdentityResult> UpdateUserAsync(T user)
    {
        user.WhenChanged = TimeProvider.GetUtcNow();
        return base.UpdateUserAsync(user);
    }

    /// <summary>
    /// 更新Person信息。已重写并添加了审计日志和拦截器。
    /// </summary>
    /// <remarks>
    /// 该方法用于更新自然人其他信息，账户有关操作不使用该方法，并且不会出发审计和拦截。请使用账户管理相关专用方法来操作账户管理任务。
    /// </remarks>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> UpdateAsync(T user)
    {
        IdentityResult result = await base.UpdateAsync(user);
        if (result.Succeeded)
            await EventService.RaiseAsync(new UpdatePersonSuccessEvent(user.UserName));
        else
            await EventService.RaiseAsync(new UpdatePersonFailureEvent(user.UserName));

        return result;
    }

    /// <summary>
    /// 已重写，删除用户。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> DeleteAsync(T user)
    {
        //正式执行删除。
        IdentityResult result = await base.DeleteAsync(user);

        if (result.Succeeded)
            await EventService.RaiseAsync(new DeletePersonSuccessEvent(user.UserName));
        else
            await EventService.RaiseAsync(new DeletePersonFailureEvent(user.UserName));

        return result;
    }

    /// <summary>
    /// 已重写，添加密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> AddPasswordAsync(T user, string password)
    {
        //检查密码历史记录
        if (HitUsedPassword(user, password))
        {
            await EventService.RaiseAsync(new ChangePasswordFailureEvent(user.UserName, "HitPasswordHistory"));
            return IdentityResult.Failed(AppErrorDescriber.ReuseOldPassword());
        }

        user.PasswordLastSet = TimeProvider.GetUtcNow();
        //记录密码历史
        LogUsedPassword(user, password);

        IdentityResult result = await base.AddPasswordAsync(user, password);
        return result;
    }

    /// <summary>
    /// 已重写。修改密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="currentPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ChangePasswordAsync(T user,
        string currentPassword,
        string newPassword)
    {
        //检查密码最小寿命。
        if (PasswordLifetime.MinimumAge > 0)
            if (user.PasswordLastSet.HasValue)
            {
                DateTimeOffset coldDownEnd = TimeProvider.GetUtcNow()
                    .AddMinutes(PasswordLifetime.MinimumAge);
                if (user.PasswordLastSet.Value > coldDownEnd)
                {
                    await EventService.RaiseAsync(new ChangePasswordFailureEvent(user.UserName, "MinimumPasswordAge"));
                    return IdentityResult.Failed(AppErrorDescriber.LessThenMinimumPasswordAge());
                }
            }

        //检查密码历史记录
        if (HitUsedPassword(user, newPassword))
        {
            await EventService.RaiseAsync(new ChangePasswordFailureEvent(user.UserName, "HitPasswordHistory"));
            return IdentityResult.Failed(AppErrorDescriber.ReuseOldPassword());
        }

        //正式进入更改密码。
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        user.PasswordLastSet = TimeProvider.GetUtcNow();
        //记录密码历史
        LogUsedPassword(user, newPassword);

        IdentityResult result = await base.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            await EventService.RaiseAsync(new ChangePasswordFailureEvent(user.UserName, "基础设施返回了错误。"));
            return result;
        }


        trans.Complete();
        await EventService.RaiseAsync(new ChangePasswordSuccessEvent(user.UserName, "用户修改了密码"));
        return result;
    }

    /// <summary>
    /// 已重写。用户重设密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ResetPasswordAsync(T user, string token, string newPassword)
    {
        //重设密码是否受密码最短寿命限制？不受最短寿命限制。
        //检查密码历史记录
        if (HitUsedPassword(user, newPassword))
        {
            await EventService.RaiseAsync(new ChangePasswordFailureEvent(user.UserName, "HitPasswordHistory"));
            return IdentityResult.Failed(AppErrorDescriber.ReuseOldPassword());
        }

        user.PasswordLastSet = TimeProvider.GetUtcNow();
        //记录密码历史
        LogUsedPassword(user, newPassword);

        IdentityResult result = await base.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            await EventService.RaiseAsync(
                new ChangePasswordFailureEvent(user.UserName, result.Errors.Select(e => e.Description)
                    .Aggregate((x, y) => $"{x},{y}")));
            return result;
        }


        await EventService.RaiseAsync(new ChangePasswordSuccessEvent(user.UserName, "用户重置了密码。"));
        return result;
    }

    /// <summary>
    /// 已重写。移除本地登录密码。
    /// 该方法还会清空<see cref="ApplicationUser.PasswordLastSet" />的值。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> RemovePasswordAsync(T user)
    {
        user.PasswordLastSet = null;
        IdentityResult result = await base.RemovePasswordAsync(user);
        if (!result.Succeeded)
        {
            await EventService.RaiseAsync(
                new ChangePasswordFailureEvent(user.UserName, result.Errors.Select(e => e.Description)
                    .Aggregate((x, y) => $"{x},{y}")));
            return result;
        }

        await EventService.RaiseAsync(new ChangePasswordSuccessEvent(user.UserName, "用户删除了密码。"));
        return result;
    }

    /// <summary>
    /// 管理员重置用户密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newPassword"></param>
    /// <param name="mustChangePassword"></param>
    /// <param name="unlockUser"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ResetPasswordAsync(T user,
        string newPassword,
        bool mustChangePassword,
        bool unlockUser)
    {
        if (mustChangePassword)
            user.PasswordLastSet = null;

        IdentityResult result = await UpdatePasswordHash(user, newPassword, true);
        if (!result.Succeeded)
            return result;

        result = await UpdateUserAsync(user);

        if (unlockUser)
            result = await UnlockUserAsync(user);
        if (!result.Succeeded)
        {
            string errMessage = result.Errors.Select(p => p.Description).Aggregate((a, b) => $"{a}, {b}");
            await EventService.RaiseAsync(new ChangePasswordFailureEvent(user.UserName, errMessage));
        }
        else
        {
            await EventService.RaiseAsync(new ChangePasswordSuccessEvent(user.UserName, "管理员重置了用户密码。"));
        }
        return result;
    }

    /// <summary>
    /// 解锁用户。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> UnlockUserAsync(T person)
    {
        if (await IsLockedOutAsync(person))
        {
            Logger.LogDebug("正在解锁用户{user}", person);
            return await SetLockoutEndDateAsync(person, null);
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 尝试设置时区。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tzName"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> SetTimeZone(T user, string tzName)
    {
        string? ianaTimeZoneName = null;
        if (TZConvert.KnownWindowsTimeZoneIds.Contains(tzName))
            ianaTimeZoneName = TZConvert.WindowsToIana(tzName);
        else if (TZConvert.KnownIanaTimeZoneNames.Contains(tzName)) ianaTimeZoneName = tzName;
        if (ianaTimeZoneName != null)
        {
            user.TimeZone = ianaTimeZoneName;
            return await UpdateAsync(user);
        }

        Logger.LogDebug("给定的时区名称{TimeZoneString}不是有效的", tzName);
        return IdentityResult.Failed(new IdentityError
        { Code = "Invalid_TzInfo", Description = "Invalid time zone name." });
    }

    /// <summary>
    /// 设置头像。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="contentType"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> SetProfilePictureAsync(T person,
        string contentType,
        byte[] bytes)
    {
        try
        {
            Image.Identify(bytes);
        }
        catch (InvalidImageContentException ex)
        {
            Logger.LogWarning(ex, "传入的数据不是有效的图片内容。");
            throw;
        }

        person.ProfilePicture = new BinaryDataInfo(contentType, bytes);
        IdentityResult result = await UpdateAsync(person);
        return result;
    }

    /// <summary>
    /// 清除用户的头像。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ClearProfilePictureAsync(T person)
    {
        person.ProfilePicture = null;
        IdentityResult result = await UpdateAsync(person);
        if (result.Succeeded)
            Logger.LogInformation("用户头像已清除。");
        else
            Logger.LogWarning("清除用户头像时不成功。{result}", result);
        return result;
    }

    /// <inheritdoc />
    public override async Task<IdentityResult> SetPhoneNumberAsync(T user, string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return await base.SetPhoneNumberAsync(user, null);

        if (!MobilePhoneNumber.TryParse(phoneNumber, out MobilePhoneNumber number))
        {
            return IdentityResult.Failed(AppErrorDescriber.InvalidPhoneNumberFormat());
        }
        return await base.SetPhoneNumberAsync(user, phoneNumber);
    }

    /// <summary>
    /// 设置指定用户的手机号，并可选地标记为已确认。
    /// </summary>
    /// <remarks>
    /// 此方法会更新用户的手机号，并根据 <paramref name="confirmed"/> 参数设置 <see cref="PhoneNumberConfirmed"/> 属性。用户对象会被持久化到底层存储。
    /// </remarks>
    /// <param name="user">要设置手机号的用户。不能为空。</param>
    /// <param name="phoneNumber">要设置的手机号。可以为 <see langword="null"/> 以移除手机号。</param>
    /// <param name="confirmed">指示手机号是否应被标记为已确认的值。</param>
    /// <returns>一个 <see cref="IdentityResult"/>，指示操作结果。成功时返回 <see cref="IdentityResult.Success"/>，否则返回错误结果。</returns>
    public virtual async Task<IdentityResult> SetPhoneNumberAsync(T user, string? phoneNumber, bool confirmed)
    {
        IdentityResult result = await this.SetPhoneNumberAsync(user, phoneNumber);
        if (!result.Succeeded) return result;

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            user.PhoneNumberConfirmed = confirmed;
            return await UpdateUserAsync(user);
        }

        return result;
    }

    /// <summary>
    /// 检查用户是否使用了已使用的密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    protected virtual bool HitUsedPassword(T user, string password)
    {
        if (passwordLifetimeOptions.Value.RememberPasswordHistory < 1)
            return false;

        var usedPasswords = user.UsedPasswords.OrderByDescending(p => p.Id)
            .Take(passwordLifetimeOptions.Value.RememberPasswordHistory).ToArray();
        foreach (var usedPassword in usedPasswords)
        {
            var result = PasswordHasher.VerifyHashedPassword(user, usedPassword.PasswordHash, password);
            if (result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                Logger.LogDebug("用户{user}使用了已使用的密码。", user);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 记录用户使用的密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    protected virtual void LogUsedPassword(T user, string password)
    {
        if (passwordLifetimeOptions.Value.RememberPasswordHistory < 1)
            return;

        //按创建日期倒排裁剪超过29条后的记录。
        var cuts = user.UsedPasswords.OrderByDescending(p => p.Id).Skip(29).ToArray();
        foreach (var cut in cuts)
        {
            user.UsedPasswords.Remove(cut);
        }
        user.UsedPasswords.Add(new UsedPassword()
        {
            PasswordHash = PasswordHasher.HashPassword(user, password),
        });
    }
}