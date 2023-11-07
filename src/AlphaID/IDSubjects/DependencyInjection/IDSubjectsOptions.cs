using Microsoft.AspNetCore.Identity;

namespace IDSubjects.DependencyInjection;
/// <summary>
/// Options for IDSubjects.
/// </summary>
public class IdSubjectsOptions : IdentityOptions
{
    /// <summary>
    /// 已替换，提供Password有关选项。
    /// </summary>
    public new IDSubjectsPasswordOptions Password { get; set; } = new();
}
