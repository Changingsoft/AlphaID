using IDSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
                                IOptions<IdentityOptions> optionsAccessor,
                                IPasswordHasher<NaturalPerson> passwordHasher,
                                IEnumerable<IUserValidator<NaturalPerson>> userValidators,
                                IEnumerable<IPasswordValidator<NaturalPerson>> passwordValidators,
                                ILookupNormalizer keyNormalizer,
                                IdentityErrorDescriber errors,
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
        this.Logger.LogInformation("用户{person}成功执行了登录，登录成功计数器+1，记录登录时间{time}，登录方式为：{authenticationMethod}", person, DateTime.Now, authenticationMethod);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 已重写。更新自然人信息。此方法将更新WhenChanged属性。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> UpdateAsync(NaturalPerson person)
    {
        person.WhenChanged = DateTime.Now;
        return await base.UpdateAsync(person);
    }



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
    public override Task<IdentityResult> CreateAsync(NaturalPerson user, string password)
    {
        user.WhenCreated = DateTime.UtcNow;
        return base.CreateAsync(user, password);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected override Task<IdentityResult> UpdateUserAsync(NaturalPerson user)
    {
        user.WhenChanged = DateTime.UtcNow;
        return base.UpdateUserAsync(user);
    }

    /// <summary>
    /// 尝试设置时区。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tzName"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> SetTimeZone(NaturalPerson user, string tzName)
    {
        if(TZConvert.KnownIanaTimeZoneNames.Any(p => p == tzName))
        {
            user.TimeZone = tzName;
            return Task.FromResult(IdentityResult.Success);
        }
        return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "Invalid_TzInfo", Description = "Invalid time zone name." }));
    }
}
