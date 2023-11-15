using Microsoft.AspNetCore.Identity;

namespace IdSubjects.DependencyInjection;
/// <summary>
/// Options for IdSubjects.
/// </summary>
public class IdSubjectsOptions : IdentityOptions
{
    /// <summary>
    /// 已替换，提供Password有关选项。
    /// </summary>
    public new IdSubjectsPasswordOptions Password { get; set; } = new();
}
