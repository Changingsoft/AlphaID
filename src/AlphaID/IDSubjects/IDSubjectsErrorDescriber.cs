using Microsoft.AspNetCore.Identity;

namespace IDSubjects;
public class IDSubjectsErrorDescriber : IdentityErrorDescriber
{
    public virtual IdentityError MembershipExists() => new()
    {
        Code = nameof(MembershipExists),
        Description = Resources.MembershipExists,
    };

    public virtual IdentityError LastOwnerCannotLeave() => new()
    {
        Code = nameof(LastOwnerCannotLeave),
        Description = Resources.LastOwnerCannotLeave,
    };
}
