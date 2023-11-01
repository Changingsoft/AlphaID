using Microsoft.AspNetCore.Identity;

namespace IDSubjects;
public class NaturalPersonIdentityErrorDescriber : IdentityErrorDescriber
{
    public virtual IdentityError InvalidPhoneNumberFormat()
    {
        return new IdentityError()
        {
            Code = nameof(InvalidPhoneNumberFormat),
            Description = Resources.InvalidPhoneNumber,
        };
    }

    public virtual IdentityError DuplicatePhoneNumber()
    {
        return new IdentityError()
        {
            Code = nameof(DuplicatePhoneNumber),
            Description = Resources.DuplicatePhoneNumber,
        };
    }
}
