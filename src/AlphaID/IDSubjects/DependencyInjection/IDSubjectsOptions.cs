using Microsoft.AspNetCore.Identity;

namespace IDSubjects.DependencyInjection;
/// <summary>
/// Options for IDSubjects.
/// </summary>
public class IdSubjectsOptions : IdentityOptions
{
    public new IDSubjectsPasswordOptions Password { get; set; } = new();
}
