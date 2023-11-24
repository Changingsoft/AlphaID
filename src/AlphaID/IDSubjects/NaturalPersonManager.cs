using IdSubjects.DependencyInjection;
using IdSubjects.Diagnostics;
using IdSubjects.SecurityAuditing;
using IdSubjects.SecurityAuditing.Events;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Transactions;
using TimeZoneConverter;

namespace IdSubjects;

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
    /// <param name="interceptors"></param>
    /// <param name="passwordHistoryManager"></param>
    /// <param name="eventService"></param>
    public NaturalPersonManager(INaturalPersonStore store,
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
        this.Interceptors = interceptors;
        this.PasswordHistoryManager = passwordHistoryManager;
        this.EventService = eventService;
        this.Options = optionsAccessor.Value;
        this.Store = store;
        this.ErrorDescriber = errors;
    }

    /// <summary>
    /// 获取或设置IdSubjectsOptions。
    /// </summary>
    public new IdSubjectsOptions Options { get; set; }

    /// <summary>
    /// 获取 INaturalPersonStore.
    /// </summary>
    public new INaturalPersonStore Store { get; }

    /// <summary>
    /// 获取拦截器。
    /// </summary>
    public IEnumerable<IInterceptor> Interceptors { get; }

    /// <summary>
    /// 
    /// </summary>
    public new NaturalPersonIdentityErrorDescriber ErrorDescriber { get; }

    /// <summary>
    /// 获取或设置时间提供器以便于可测试性。
    /// </summary>
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 获取用于密码历史的管理器。
    /// </summary>
    public PasswordHistoryManager PasswordHistoryManager { get; }

    /// <summary>
    /// 获取审计事件服务。
    /// </summary>
    protected IEventService EventService { get; }

    /// <summary>
    /// 通过移动电话号码查找自然人。
    /// </summary>
    /// <param name="mobile">移动电话号码，支持不带国际区号的11位号码格式或标准E.164格式。</param>
    /// <param name="cancellation"></param>
    /// <returns>返回找到的自然人。如果没有找到，则返回null。</returns>
    public virtual async Task<NaturalPerson?> FindByMobileAsync(string mobile, CancellationToken cancellation)
    {
        if (!MobilePhoneNumber.TryParse(mobile, out var phoneNumber))
            return null;
        var phoneNumberString = phoneNumber.ToString();
        var person = await this.Store.FindByPhoneNumberAsync(phoneNumberString, cancellation);
        return person;
    }

    /// <summary>
    /// 获取 Natural Person 的原始未更改版本。
    /// 此方法相当于从存取器获取位于持久化基础结构中的没有更改的原始版本。
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public virtual Task<NaturalPerson> GetOriginalAsync(NaturalPerson current)
    {
        return Task.FromResult(this.Store.Users.Single(p => p.Id == current.Id));
    }

    /// <summary>
    /// 已重写。创建用户。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> CreateAsync(NaturalPerson user)
    {
        var utcNow = this.TimeProvider.GetUtcNow();
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        user.PersonWhenChanged = utcNow;
        var result = await base.CreateAsync(user);
        if (result.Succeeded)
        {
            await this.EventService.RaiseAsync(new CreatePersonSuccessEvent());
        }
        else
        {
            await this.EventService.RaiseAsync(new CreatePersonFailureEvent());
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> CreateAsync(NaturalPerson user, string password)
    {
        var utcNow = this.TimeProvider.GetUtcNow();
        user.WhenCreated = utcNow;
        user.WhenChanged = utcNow;
        user.PersonWhenChanged = utcNow;
        user.PasswordLastSet = utcNow;
        var result = await base.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await this.EventService.RaiseAsync(new CreatePersonSuccessEvent());
        }
        else
        {
            await this.EventService.RaiseAsync(new CreatePersonFailureEvent());
        }

        return result;
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
        this.Logger.LogInformation("用户{person}成功执行了登录，登录成功计数器+1，记录登录时间{time}，登录方式为：{authenticationMethod}", person, this.TimeProvider.GetUtcNow(), authenticationMethod);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 已重写。若用户实现不对账户相关字段变更时，不应在用户实现中调用该方法。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected override async Task<IdentityResult> UpdateUserAsync(NaturalPerson user)
    {
        user.WhenChanged = this.TimeProvider.GetUtcNow();
        return await base.UpdateUserAsync(user);
    }

    /// <summary>
    /// 更新Person信息。已重写并添加了审计日志和拦截器。
    /// </summary>
    /// <remarks>
    /// 该方法用于更新自然人其他信息，账户有关操作不使用该方法，并且不会出发审计和拦截。请使用账户管理相关专用方法来操作账户管理任务。
    /// </remarks>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> UpdateAsync(NaturalPerson user)
    {
        List<IdentityError> errors = new();
        bool passPreUpdate = true;
        var stack = new Stack<INaturalPersonInterceptor>();
        foreach (var interceptor in this.Interceptors.OfType<INaturalPersonInterceptor>())
        {
            stack.Push(interceptor);
            var interceptResult = await interceptor.PreUpdateAsync(this, user);
            if (!interceptResult.Succeeded)
                passPreUpdate = false;
            errors.AddRange(interceptResult.Errors);
        }
        if (!passPreUpdate)
            return IdentityResult.Failed(errors.ToArray());

        user.PersonWhenChanged = this.TimeProvider.GetUtcNow();
        var result = await base.UpdateAsync(user);
        if (result.Succeeded)
        {
            await this.EventService.RaiseAsync(new UpdatePersonSuccessEvent());
        }
        else
        {
            await this.EventService.RaiseAsync(new UpdatePersonFailureEvent());
        }

        while (stack.TryPop(out var interceptor))
        {
            await interceptor.PostUpdateAsync(this, user);
        }
        return result;
    }

    /// <summary>
    /// 已重写，删除用户。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> DeleteAsync(NaturalPerson user)
    {
        bool passPreAction = true;
        List<IdentityError> errors = new();
        Stack<INaturalPersonInterceptor> stack = new();
        foreach (var inceptor in this.Interceptors.OfType<INaturalPersonInterceptor>())
        {
            stack.Push(inceptor);
            var inceptorResult = await inceptor.PreDeleteAsync(this, user);
            if (!inceptorResult.Succeeded)
                passPreAction = false;
            errors.AddRange(inceptorResult.Errors);
        }
        if (!passPreAction)
            return IdentityResult.Failed(errors.ToArray());

        //正式执行删除。
        var result = await base.DeleteAsync(user);

        if (result.Succeeded)
        {
            await this.EventService.RaiseAsync(new DeletePersonSuccessEvent());
        }
        else
        {
            await this.EventService.RaiseAsync(new DeletePersonFailureEvent());
        }

        while (stack.TryPop(out var interceptor))
        {
            await interceptor.PostDeleteAsync(this, user);
        }
        return result;
    }

    /// <summary>
    /// 已重写，添加密码。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> AddPasswordAsync(NaturalPerson user, string password)
    {
        user.PasswordLastSet = this.TimeProvider.GetUtcNow();
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
    public override async Task<IdentityResult> ChangePasswordAsync(NaturalPerson user, string currentPassword, string newPassword)
    {
        //检查最短有效期。
        if (this.Options.Password.ChangePasswordColdDown > 0)
        {
            if (user.PasswordLastSet.HasValue)
            {
                var coldDownEnd = this.TimeProvider.GetUtcNow()
                    .AddMinutes(this.Options.Password.ChangePasswordColdDown);
                if (user.PasswordLastSet.Value > coldDownEnd)
                {
                    await this.EventService.RaiseAsync(new ChangePasswordFailureEvent("MinimumPasswordAge"));
                    return IdentityResult.Failed(this.ErrorDescriber.LessThenMinimumPasswordAge());
                }
            }
        }

        //检查密码历史记录
        if (this.Options.Password.RememberPasswordHistory > 0)
        {
            if (this.PasswordHistoryManager.Hit(user, newPassword))
            {
                await this.EventService.RaiseAsync(new ChangePasswordFailureEvent("HitPasswordHistory"));
                return IdentityResult.Failed(this.ErrorDescriber.ReuseOldPassword());
            }
        }

        //执行拦截器。
        Stack<IUserPasswordInterceptor> interceptorStack = new();
        List<IdentityError> errors = new();
        bool passInterceptors = true;
        foreach (var interceptor in this.Interceptors.OfType<IUserPasswordInterceptor>())
        {
            interceptorStack.Push(interceptor);
            var interceptorResult = await interceptor.PasswordChangingAsync(user, newPassword, CancellationToken.None);
            if (!interceptorResult.Succeeded)
                passInterceptors = false;
            errors.AddRange(interceptorResult.Errors);
        }
        if (!passInterceptors)
        {
            await this.EventService.RaiseAsync(new ChangePasswordFailureEvent("Canceled by interceptors."));
            return IdentityResult.Failed(errors.ToArray());
        }

        //正式进入更改密码。
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        user.PasswordLastSet = this.TimeProvider.GetUtcNow();
        IdentityResult result = await base.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            await this.EventService.RaiseAsync(new ChangePasswordFailureEvent("基础设施返回了错误。"));
            return result;
        }

        //执行拦截器。
        while (interceptorStack.TryPop(out var interceptor))
        {
            await interceptor.PasswordChangedAsync(user, CancellationToken.None);
        }

        //记录密码历史
        if (this.Options.Password.RememberPasswordHistory > 0)
            await this.PasswordHistoryManager.Pass(user, newPassword);

        await this.EventService.RaiseAsync(new ChangePasswordSuccessEvent("基础设施返回了错误。"));
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
        user.PasswordLastSet = null;
        IdentityResult result = await base.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            await this.EventService.RaiseAsync(new ChangePasswordFailureEvent(result.Errors.ToString()));
        }
        else
        {
            await this.EventService.RaiseAsync(new ChangePasswordSuccessEvent("用户重置了密码。"));
        }
        return result;
    }

    /// <summary>
    /// 已重写。移除本地登录密码。
    /// 该方法还会清空<see cref="NaturalPerson.PasswordLastSet"/>的值。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> RemovePasswordAsync(NaturalPerson user)
    {
        user.PasswordLastSet = null;
        IdentityResult result = await base.RemovePasswordAsync(user);
        if (!result.Succeeded)
        {
            await this.EventService.RaiseAsync(new ChangePasswordFailureEvent(result.Errors.ToString()));
        }
        else
        {
            await this.EventService.RaiseAsync(new ChangePasswordSuccessEvent("用户删除了密码。"));
        }
        return result;
    }

    /// <summary>
    /// 强制更改用户的姓名信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="personName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AdminChangePersonNameAsync(NaturalPerson person, PersonNameInfo personName)
    {
        person.PersonName = personName;
        return await this.UpdateAsync(person);
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
        if (mustChangePassword)
        {
            person.PasswordLastSet = null;
        }
        IdentityResult result = await this.UpdatePasswordHash(person, newPassword, true);
        if (!result.Succeeded)
            return result;

        if (unlockUser)
            result = await this.UnlockPersonAsync(person);
        if (!result.Succeeded)
        {
            await this.EventService.RaiseAsync(new ChangePasswordFailureEvent(result.Errors.ToString()));
        }
        else
        {
            await this.EventService.RaiseAsync(new ChangePasswordSuccessEvent("管理员重置了用户密码。"));
        }
        return result;
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
            //todo 更新持久化？
            return Task.FromResult(IdentityResult.Success);
        }
        return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "Invalid_TzInfo", Description = "Invalid time zone name." }));
    }

    /// <summary>
    /// 设置头像。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="contentType"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> SetProfilePictureAsync(NaturalPerson person, string contentType, byte[] bytes)
    {
        try
        {
            Image.Identify(bytes);
        }
        catch (InvalidImageContentException ex)
        {
            this.Logger.LogWarning(ex, "传入的数据不是有效的图片内容。");
            throw;
        }

        person.ProfilePicture = new BinaryDataInfo(contentType, bytes);
        var result = await this.UpdateAsync(person);
        return result;
    }

    /// <summary>
    /// 清除用户的头像。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ClearProfilePictureAsync(NaturalPerson person)
    {
        person.ProfilePicture = null;
        var result = await this.UpdateAsync(person);
        if (result.Succeeded)
            this.Logger.LogInformation("用户头像已清除。");
        else
            this.Logger.LogWarning("清除用户头像时不成功。{result}", result);
        return result;
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> SetPhoneNumberAsync(NaturalPerson user, string? phoneNumber)
    {
        var result = await base.SetPhoneNumberAsync(user, phoneNumber);
        if (!result.Succeeded)
        {
            return result;
        }

        //todo 考虑从选项来控制是否自动将PhoneNumberConfirmed设置为true
        user.PhoneNumberConfirmed = true;
        return await this.UpdateUserAsync(user);

    }
}