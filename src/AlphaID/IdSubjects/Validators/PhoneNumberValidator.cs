using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Validators;

/// <summary>
/// 手机号验证器。
/// </summary>
public class PhoneNumberValidator<T> : IUserValidator<T>
where T : ApplicationUser
{
    /// <summary>
    /// 添加对手机号格式和唯一性的验证。
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual async Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user)
    {
        ApplicationUserManager<T> applicationUserManager = manager as ApplicationUserManager<T> ??
                                                    throw new InvalidOperationException(
                                                        "无法转换UserManger到ApplicationUserManager。");
        var errors = new List<IdentityError>();
        string? phoneNumber = await applicationUserManager.GetPhoneNumberAsync(user);
        if (phoneNumber == null) return errors.Count > 0 ? IdentityResult.Failed([.. errors]) : IdentityResult.Success;

        if (MobilePhoneNumber.TryParse(phoneNumber, out MobilePhoneNumber number))
        {
            ApplicationUser? owner =
                await applicationUserManager.FindByMobileAsync(number.ToString());
            if (owner != null && !string.Equals(user.Id, owner.Id))
                //手机号已存在且不隶属该用户，则提示手机号重复。
                errors.Add(applicationUserManager.AppErrorDescriber.DuplicatePhoneNumber());
        }
        else
        {
            errors.Add(applicationUserManager.AppErrorDescriber.InvalidPhoneNumberFormat());
        }

        return errors.Count > 0 ? IdentityResult.Failed([.. errors]) : IdentityResult.Success;
    }
}