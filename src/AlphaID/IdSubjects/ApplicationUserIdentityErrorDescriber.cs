using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
/// Error describer. 便于国际化。
/// </summary>
public class ApplicationUserIdentityErrorDescriber : IdentityErrorDescriber
{
    /// <summary>
    /// 无效的移动电话号码格式。
    /// </summary>
    /// <returns></returns>
    public virtual IdentityError InvalidPhoneNumberFormat()
    {
        return new IdentityError
        {
            Code = nameof(InvalidPhoneNumberFormat),
            Description = Resources.InvalidPhoneNumber
        };
    }

    /// <summary>
    /// 电话号码重复。
    /// </summary>
    /// <returns></returns>
    public virtual IdentityError DuplicatePhoneNumber()
    {
        return new IdentityError
        {
            Code = nameof(DuplicatePhoneNumber),
            Description = Resources.DuplicatePhoneNumber
        };
    }

    /// <summary>
    /// CannotChangePersonName
    /// </summary>
    /// <returns></returns>
    public IdentityError CannotChangePersonName()
    {
        return new IdentityError
        {
            Code = nameof(CannotChangePersonName), Description = Resources.CannotChangePersonName
        };
    }

    /// <summary>
    /// 低于密码最小生存期。
    /// </summary>
    /// <returns></returns>
    public IdentityError LessThenMinimumPasswordAge()
    {
        return new IdentityError
        {
            Code = nameof(LessThenMinimumPasswordAge),
            Description = Resources.LessThenMinimumPasswordAge
        };
    }

    /// <summary>
    /// 重复使用了旧密码。
    /// </summary>
    /// <returns></returns>
    public IdentityError ReuseOldPassword()
    {
        return new IdentityError
        {
            Code = nameof(ReuseOldPassword),
            Description = Resources.ReuseOldPassword
        };
    }

    /// <summary>
    /// 密码需要包含大写、小写、数字和特殊字符四种中的至少三种。
    /// </summary>
    /// <returns></returns>
    public IdentityError PasswordRequires3Of4()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequires3Of4),
            Description = Resources.PasswordRequires3Of4
        };
    }

    /// <inheritdoc />
    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = Resources.PasswordRequiresDigit,
        };
    }

    /// <inheritdoc />
    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError()
        {
            Code = nameof(PasswordRequiresLower),
            Description = Resources.PasswordRequiresLower,
        }; 
    }

    /// <inheritdoc />
    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = Resources.PasswordRequiresUpper,
        };
    }

    /// <inheritdoc />
    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = Resources.PasswordRequiresNonAlphanumeric,
        };
    }

    /// <inheritdoc />
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError()
        {
            Code = nameof(PasswordTooShort),
            Description = Resources.PasswordTooShort,
        };
    }

    /// <inheritdoc />
    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new IdentityError()
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = Resources.PasswordRequiresUniqueChars,
        };
    }
}