using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Validators;

/// <summary>
/// 一个密码验证器，他要求密码必须达到规定的长度，并且满足至少三种字符类型的要求。
/// </summary>
/// <typeparam name="TUser">表示User的类型。</typeparam>
/// <param name="errorDescriber">错误描述符。</param>
public class IdSubjectsPasswordValidator<TUser>(IdentityErrorDescriber errorDescriber) : PasswordValidator<TUser>(errorDescriber) where TUser : ApplicationUser
{
    /// <inheritdoc />
    public override Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        ArgumentNullException.ThrowIfNull(manager, nameof(manager));
        var describer = Describer as ApplicationUserIdentityErrorDescriber ?? throw new InvalidOperationException("没有注册ApplicationUserIdentityErrorDescriber");

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
            errors.Add(describer.PasswordRequires3Of4());
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
