using System.Transactions;
using IdSubjects.DependencyInjection;
using IdSubjects.Diagnostics;
using IdSubjects.SecurityAuditing;
using IdSubjects.SecurityAuditing.Events;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using TimeZoneConverter;

namespace IdSubjects;

/// <summary>
///     自然人管理器。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="store"></param>
/// <param name="optionsAccessor"></param>
/// <param name="passwordHasher"></param>
/// <param name="userValidators"></param>
/// <param name="passwordValidators"></param>
/// <param name="keyNormalizer"></param>
/// <param name="errors"></param>
/// <param name="services"></param>
/// <param name="logger"></param>
/// <param name="interceptors"></param>
/// <param name="passwordHistoryManager"></param>
/// <param name="eventService"></param>
public class NaturalPersonManager(
    INaturalPersonStore store,
    IOptions<IdSubjectsOptions> optionsAccessor,
    IPasswordHasher<NaturalPerson> passwordHasher,
    IEnumerable<IUserValidator<NaturalPerson>> userValidators,
    IEnumerable<IPasswordValidator<NaturalPerson>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    NaturalPersonIdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<NaturalPersonManager> logger,
    IEnumerable<IInterceptor> interceptors,
    PasswordHistoryManager passwordHistoryManager,
    IEventService eventService)
    : UserManager<NaturalPerson>(store,
        optionsAccessor,
        passwordHasher,
        userValidators,
        passwordValidators,
        keyNormalizer,
        errors,
        services,
        logger)
{
    /// <summary>
    ///     获取或设置IdSubjectsOptions。
    /// </summary>
    public new IdSubjectsOptions Options { get; set; } = optionsAccessor.Value;

    /// <summary>
    ///     获取 INaturalPersonStore.
    /// </summary>
    public new INaturalPersonStore Store { get; } = store;

    /// <summary>
    ///     获取拦截器。
    /// </summary>
    public IEnumerable<IInterceptor> Interceptors { get; } = interceptors;

    /// <summary>
    /// </summary>
    public new NaturalPersonIdentityErrorDescriber ErrorDescriber { get; } = errors;

    /// <summary>
    ///     获取或设置时间提供器以便于可测试性。
    /// </summary>
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    ///     获取用于密码历史的管理器。
    /// </summary>
    public PasswordHistoryManager PasswordHistoryManager { get; } = passwordHistoryManager;

    /// <summary>
    ///     获取审计事件服务。
    /// </summary>
    protected IEventService EventService { get; } = eventService;

    /// <summary>
    ///     通过移动电话号码查找自然人。
    /// </summary>
    /// <param name="mobile">移动电话号码，支持不带国际区号的11位号码格式或标准 E.164 格式。</param>
    /// <param name="cancellation"></param>
    /// <returns>返回找到的自然人。如果没有找到，则返回null。</returns>
    public virtual async Task<NaturalPerson?> FindByMobileAsync(string mobile, CancellationToken cancellation)
    {
        if (!MobilePhoneNumber.TryParse(mobile, out MobilePhoneNumber phoneNumber))
            return null;
        var phoneNumberString = phoneNumber.ToString();
        NaturalPerson? person = await Store.FindByPhoneNumberAsync(phoneNumberString, cancellation);
        return person;
    }

    /// <summary>
    ///     获取 Natural Person 的原始未更改版本。
    ///     此方法相当于从存取器获取位于持久化基础结构中的没有更改的原始版本。
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public virtual Task<NaturalPerson?> GetOriginalAsync(NaturalPerson current)
    {
        return Store.GetOriginalAsync(current, CancellationToken.None);
    }

    /// <summary>
    ///     已重写。创建用户。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> CreateAsync(NaturalPerson user)
    {
        var aggregator =
            new NaturalPersonCreateInterceptorAggregator(Interceptors.OfType<INaturalPersonCreateInterceptor>());
        IdentityResult preActionResult = await aggregator.PreCreate(this, user);
        if (!preActionResult.Succeeded) return preActionResult;
        DateTimeOffset utcNow = TimeProvider.GetUtcNow();
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        user.PersonWhenChanged = utcNow;
        IdentityResult result = await base.CreateAsync(user);
        if (result.Succeeded)
            await EventService.RaiseAsync(new CreatePersonSuccessEvent());
        else
            await EventService.RaiseAsync(new CreatePersonFailureEvent());

        await aggregator.PostCreate(this, user);
        return result;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> CreateAsync(NaturalPerson user, string password)
    {
        var aggregator =
            new NaturalPersonCreateInterceptorAggregator(Interceptors.OfType<INaturalPersonCreateInterceptor>());
        IdentityResult preActionResult = await aggregator.PreCreate(this, user, password);
        if (!preActionResult.Succeeded) return preActionResult;
        DateTimeOffset utcNow = TimeProvider.GetUtcNow();
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        user.PersonWhenChanged = utcNow;
        user.PasswordLastSet = utcNow;
        IdentityResult result = await base.CreateAsync(user, password);
        if (result.Succeeded)
            await EventService.RaiseAsync(new CreatePersonSuccessEvent());
        else
            await EventService.RaiseAsync(new CreatePersonFailureEvent());

        await aggregator.PostCreate(this, user);
        return result;
    }

    /// <summary>
    ///     当身份验证成功时，调用此方法以记录包括登录次数、上次登录时间、登录方式等信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="authenticationMethod"></param>
    /// <returns></returns>
    public virtual Task AccessSuccededAsync(NaturalPerson person, string authenticationMethod)
    {
        //todo 记录任何登录成功次数、上次登录时间，登录方式，登录IP等。
        Logger.LogInformation("用户{person}成功执行了登录，登录成功计数器+1，记录登录时间{time}，登录方式为：{authenticationMethod}", person,
            TimeProvider.GetUtcNow(), authenticationMethod);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     已重写。若用户实现不对账户相关字段变更时，不应在用户实现中调用该方法。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected override Task<IdentityResult> UpdateUserAsync(NaturalPerson user)
    {
        user.WhenChanged = TimeProvider.GetUtcNow();
        return base.UpdateUserAsync(user);
    }

    /// <summary>
    ///     更新Person信息。已重写并添加了审计日志和拦截器。
    /// </summary>
    /// <remarks>
    ///     该方法用于更新自然人其他信息，账户有关操作不使用该方法，并且不会出发审计和拦截。请使用账户管理相关专用方法来操作账户管理任务。
    /// </remarks>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> UpdateAsync(NaturalPerson user)
    {
        NaturalPersonUpdateInterceptorAggregator aggregator =
            new(Interceptors.OfType<INaturalPersonUpdateInterceptor>());

        IdentityResult preUpdateResult = await aggregator.PreUpdateAsync(this, user);
        if (!preUpdateResult.Succeeded) return preUpdateResult;

        user.PersonWhenChanged = TimeProvider.GetUtcNow();
        IdentityResult result = await base.UpdateAsync(user);
        if (result.Succeeded)
            await EventService.RaiseAsync(new UpdatePersonSuccessEvent());
        else
            await EventService.RaiseAsync(new UpdatePersonFailureEvent());

        await aggregator.PostUpdateAsync(this, user);
        return result;
    }

    /// <summary>
    ///     已重写，删除用户。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> DeleteAsync(NaturalPerson user)
    {
        var passPreAction = true;
        List<IdentityError> errors = [];
        Stack<INaturalPersonDeleteInterceptor> stack = new();
        foreach (INaturalPersonDeleteInterceptor interceptor in Interceptors.OfType<INaturalPersonDeleteInterceptor>())
        {
            stack.Push(interceptor);
            IdentityResult interceptorResult = await interceptor.PreDeleteAsync(this, user);
            if (!interceptorResult.Succeeded)
                passPreAction = false;
            errors.AddRange(interceptorResult.Errors);
        }

        if (!passPreAction)
            return IdentityResult.Failed([.. errors]);

        //正式执行删除。
        IdentityResult result = await base.DeleteAsync(user);

        if (result.Succeeded)
            await EventService.RaiseAsync(new DeletePersonSuccessEvent());
        else
            await EventService.RaiseAsync(new DeletePersonFailureEvent());

        while (stack.TryPop(out INaturalPersonDeleteInterceptor? interceptor))
            await interceptor.PostDeleteAsync(this, user);
        return result;
    }

    /// <summary>
    ///     已重写，添加密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> AddPasswordAsync(NaturalPerson user, string password)
    {
        var interceptor = new AggregatedUserPasswordInterceptor(Interceptors.OfType<IUserPasswordInterceptor>());
        IdentityResult interceptorResult =
            await interceptor.PasswordChangingAsync(user, password, CancellationToken.None);
        if (!interceptorResult.Succeeded)
            return interceptorResult;

        //检查密码历史记录
        if (Options.Password.RememberPasswordHistory > 0)
            if (PasswordHistoryManager.Hit(user, password))
            {
                await EventService.RaiseAsync(new ChangePasswordFailureEvent("HitPasswordHistory"));
                return IdentityResult.Failed(ErrorDescriber.ReuseOldPassword());
            }

        user.PasswordLastSet = TimeProvider.GetUtcNow();
        IdentityResult result = await base.AddPasswordAsync(user, password);

        //记录密码历史
        if (Options.Password.RememberPasswordHistory > 0)
            await PasswordHistoryManager.Pass(user, password);

        await interceptor.PasswordChangedAsync(user, CancellationToken.None);
        return result;
    }

    /// <summary>
    ///     已重写。修改密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="currentPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ChangePasswordAsync(NaturalPerson user,
        string currentPassword,
        string newPassword)
    {
        var interceptor = new AggregatedUserPasswordInterceptor(Interceptors.OfType<IUserPasswordInterceptor>());
        IdentityResult interceptorResult =
            await interceptor.PasswordChangingAsync(user, newPassword, CancellationToken.None);
        if (!interceptorResult.Succeeded)
            return interceptorResult;

        //检查密码最小寿命。
        if (Options.Password.MinimumAge > 0)
            if (user.PasswordLastSet.HasValue)
            {
                DateTimeOffset coldDownEnd = TimeProvider.GetUtcNow()
                    .AddMinutes(Options.Password.MinimumAge);
                if (user.PasswordLastSet.Value > coldDownEnd)
                {
                    await EventService.RaiseAsync(new ChangePasswordFailureEvent("MinimumPasswordAge"));
                    return IdentityResult.Failed(ErrorDescriber.LessThenMinimumPasswordAge());
                }
            }

        //检查密码历史记录
        if (Options.Password.RememberPasswordHistory > 0)
            if (PasswordHistoryManager.Hit(user, newPassword))
            {
                await EventService.RaiseAsync(new ChangePasswordFailureEvent("HitPasswordHistory"));
                return IdentityResult.Failed(ErrorDescriber.ReuseOldPassword());
            }

        //正式进入更改密码。
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        user.PasswordLastSet = TimeProvider.GetUtcNow();
        IdentityResult result =
            await base.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            await EventService.RaiseAsync(new ChangePasswordFailureEvent("基础设施返回了错误。"));
            return result;
        }

        //记录密码历史
        if (Options.Password.RememberPasswordHistory > 0)
            await PasswordHistoryManager.Pass(user, newPassword);

        try
        {
            await interceptor.PasswordChangedAsync(user, CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        trans.Complete();
        return result;
    }

    /// <summary>
    ///     已重写。用户重设密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ResetPasswordAsync(NaturalPerson user, string token, string newPassword)
    {
        var interceptor = new AggregatedUserPasswordInterceptor(Interceptors.OfType<IUserPasswordInterceptor>());
        IdentityResult interceptorResult =
            await interceptor.PasswordChangingAsync(user, newPassword, CancellationToken.None);
        if (!interceptorResult.Succeeded)
            return interceptorResult;

        //重设密码是否受密码最短寿命限制？不受最短寿命限制。
        //检查密码历史记录
        if (Options.Password.RememberPasswordHistory > 0)
            if (PasswordHistoryManager.Hit(user, newPassword))
            {
                await EventService.RaiseAsync(new ChangePasswordFailureEvent("HitPasswordHistory"));
                return IdentityResult.Failed(ErrorDescriber.ReuseOldPassword());
            }

        user.PasswordLastSet = TimeProvider.GetUtcNow();
        IdentityResult result = await base.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            await EventService.RaiseAsync(
                new ChangePasswordFailureEvent(result.Errors.Select(e => e.Description)
                    .Aggregate((x, y) => $"{x},{y}")));
            return result;
        }

        //记录密码历史
        if (Options.Password.RememberPasswordHistory > 0)
            await PasswordHistoryManager.Pass(user, newPassword);

        await EventService.RaiseAsync(new ChangePasswordSuccessEvent("用户重置了密码。"));
        await interceptor.PasswordChangedAsync(user, CancellationToken.None);
        return result;
    }

    /// <summary>
    ///     已重写。移除本地登录密码。
    ///     该方法还会清空<see cref="NaturalPerson.PasswordLastSet" />的值。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> RemovePasswordAsync(NaturalPerson user)
    {
        var interceptor = new AggregatedUserPasswordInterceptor(Interceptors.OfType<IUserPasswordInterceptor>());
        IdentityResult interceptorResult = await interceptor.PasswordChangingAsync(user, null, CancellationToken.None);
        if (!interceptorResult.Succeeded)
            return interceptorResult;

        user.PasswordLastSet = null;
        IdentityResult result = await base.RemovePasswordAsync(user);
        if (!result.Succeeded)
        {
            await EventService.RaiseAsync(
                new ChangePasswordFailureEvent(result.Errors.Select(e => e.Description)
                    .Aggregate((x, y) => $"{x},{y}")));
            return result;
        }

        await EventService.RaiseAsync(new ChangePasswordSuccessEvent("用户删除了密码。"));
        await interceptor.PasswordChangedAsync(user, CancellationToken.None);
        return result;
    }

    /// <inheritdoc />
    protected override async Task<IdentityResult> UpdatePasswordHash(NaturalPerson user,
        string newPassword,
        bool validatePassword)
    {
        var interceptor = new AggregatedUserPasswordInterceptor(Interceptors.OfType<IUserPasswordInterceptor>());
        IdentityResult interceptorResult =
            await interceptor.PasswordChangingAsync(user, newPassword, CancellationToken.None);
        if (!interceptorResult.Succeeded)
            return interceptorResult;

        IdentityResult result = await base.UpdatePasswordHash(user, newPassword, validatePassword);

        await interceptor.PasswordChangedAsync(user, CancellationToken.None);
        return result;
    }

    /// <summary>
    ///     强制更改用户的姓名信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="personName"></param>
    /// <returns></returns>
    public Task<IdentityResult> AdminChangePersonNameAsync(NaturalPerson person, PersonNameInfo personName)
    {
        person.PersonName = personName;
        return UpdateAsync(person);
    }


    /// <summary>
    ///     管理员重置用户密码。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="newPassword"></param>
    /// <param name="mustChangePassword"></param>
    /// <param name="unlockUser"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> AdminResetPasswordAsync(NaturalPerson person,
        string newPassword,
        bool mustChangePassword,
        bool unlockUser)
    {
        var interceptor = new AggregatedUserPasswordInterceptor(Interceptors.OfType<IUserPasswordInterceptor>());
        IdentityResult interceptorResult =
            await interceptor.PasswordChangingAsync(person, newPassword, CancellationToken.None);
        if (!interceptorResult.Succeeded)
            return interceptorResult;

        if (mustChangePassword) person.PasswordLastSet = null;
        IdentityResult result = await UpdatePasswordHash(person, newPassword, true);
        if (!result.Succeeded)
            return result;

        if (unlockUser)
            result = await UnlockUserAsync(person);
        if (!result.Succeeded)
        {
            string errMessage = result.Errors.Select(p => p.Description).Aggregate((a, b) => $"{a}, {b}");
            await EventService.RaiseAsync(new ChangePasswordFailureEvent(errMessage));
        }
        else
        {
            await EventService.RaiseAsync(new ChangePasswordSuccessEvent("管理员重置了用户密码。"));
        }

        await interceptor.PasswordChangedAsync(person, CancellationToken.None);
        return result;
    }

    /// <summary>
    ///     解锁用户。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> UnlockUserAsync(NaturalPerson person)
    {
        Logger.LogDebug("正在解锁用户{user}", person);
        if (await IsLockedOutAsync(person)) return await SetLockoutEndDateAsync(person, null);
        Logger.LogDebug("用户{user}未锁定，此操作无效果。", person);
        return IdentityResult.Success;
    }

    /// <summary>
    ///     尝试设置时区。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tzName"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> SetTimeZone(NaturalPerson user, string tzName)
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
    ///     设置头像。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="contentType"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> SetProfilePictureAsync(NaturalPerson person,
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
    ///     清除用户的头像。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ClearProfilePictureAsync(NaturalPerson person)
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
    public override async Task<IdentityResult> SetPhoneNumberAsync(NaturalPerson user, string? phoneNumber)
    {
        IdentityResult result = await base.SetPhoneNumberAsync(user, phoneNumber);
        if (!result.Succeeded) return result;

        //todo 考虑从选项来控制是否自动将PhoneNumberConfirmed设置为true
        user.PhoneNumberConfirmed = true;
        return await UpdateUserAsync(user);
    }
}