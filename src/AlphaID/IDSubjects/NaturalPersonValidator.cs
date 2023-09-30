using IDSubjects.Subjects;
using Microsoft.AspNetCore.Identity;

namespace IDSubjects;

/// <summary>
/// 
/// </summary>
public class NaturalPersonValidator : UserValidator<NaturalPerson>
{
    /// <summary>
    /// 已重写，添加对移动电话号码格式和唯一性的验证。
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public override async Task<IdentityResult> ValidateAsync(UserManager<NaturalPerson> manager, NaturalPerson user)
    {
        var errors = new List<IdentityError>();
        if (user.Mobile != null)
        {
            if (MobilePhoneNumber.TryParse(user.Mobile, out var phoneNumber))
            {
                if (manager.Users.Any(p => p.Mobile == phoneNumber.ToString() && p.Id != user.Id))
                {
                    errors.Add(new IdentityError()
                    {
                        Code = "DuplicatePhoneNumber",
                        Description = Resources.ResourceManager.GetString("DuplicatePhoneNumber", Resources.Culture)!,
                    });
                }
            }
            else
            {
                errors.Add(new IdentityError()
                {
                    Code = "InvalidPhoneNumber",
                    Description = Resources.ResourceManager.GetString("InvalidPhoneNumber", Resources.Culture)!,
                });
            }
        }

        var result = await base.ValidateAsync(manager, user);

        errors.AddRange(result.Errors);
        return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }
}
