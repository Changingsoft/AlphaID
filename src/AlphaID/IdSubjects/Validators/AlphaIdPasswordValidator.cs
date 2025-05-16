using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Validators;
public class AlphaIdPasswordValidator<TUser>(IdentityErrorDescriber errorDescriber) : PasswordValidator<TUser>(errorDescriber) where TUser : ApplicationUser
{
    ApplicationUserIdentityErrorDescriber Describer => errorDescriber as ApplicationUserIdentityErrorDescriber ?? throw new ArgumentNullException(nameof(errorDescriber));

    public override Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        ArgumentNullException.ThrowIfNull(manager, nameof(manager));

        var errors = new List<IdentityError>();
        var options = manager.Options.Password;

        if (string.IsNullOrWhiteSpace(password) || password.Length < options.RequiredLength)
        {
            errors.Add(Describer.PasswordTooShort(options.RequiredLength));
        }
        //密码应包含大写、小写、数字和特殊字符四种中的至少三种。
        int requiredCount = 0;
        if (password.Any(IsDigit))
        {
            requiredCount++;
        }
        if (password.Any(IsLower))
        {
            requiredCount++;
        }
        if (password.Any(IsUpper))
        {
            requiredCount++;
        }
        if (!password.All(IsLetterOrDigit))
        {
            requiredCount++;
        }
        if(requiredCount < 3)
        {
            errors.Add(Describer.PasswordRequires3Of4());
        }

        if (options.RequiredUniqueChars >= 1 && password.Distinct().Count() < options.RequiredUniqueChars)
        {
            errors.Add(Describer.PasswordRequiresUniqueChars(options.RequiredUniqueChars));
        }

        return
            Task.FromResult(errors.Count > 0
                ? IdentityResult.Failed([.. errors])
                : IdentityResult.Success);
    }
}
