using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
///     Error describer. 便于国际化。
/// </summary>
public class NaturalPersonIdentityErrorDescriber : IdentityErrorDescriber
{
    /// <summary>
    ///     Invalid phone number format.
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
    ///     Duplicate phone number.
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
    ///     CannotChangePersonName
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
    ///     LessThenMinimumPasswordAge
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
    ///     ReuseOldPassword
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
}