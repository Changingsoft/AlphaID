using Microsoft.AspNetCore.Identity;

namespace IDSubjects;

/// <summary>
/// Error describer. 便于国际化。
/// </summary>
public class NaturalPersonIdentityErrorDescriber : IdentityErrorDescriber
{
    /// <summary>
    /// Invalid phone number format.
    /// </summary>
    /// <returns></returns>
    public virtual IdentityError InvalidPhoneNumberFormat()
    {
        return new IdentityError()
        {
            Code = nameof(InvalidPhoneNumberFormat),
            Description = Resources.InvalidPhoneNumber,
        };
    }

    /// <summary>
    /// Duplicate phone number.
    /// </summary>
    /// <returns></returns>
    public virtual IdentityError DuplicatePhoneNumber()
    {
        return new IdentityError()
        {
            Code = nameof(DuplicatePhoneNumber),
            Description = Resources.DuplicatePhoneNumber,
        };
    }
}
