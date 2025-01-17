using IdSubjects.SecurityAuditing;
using Microsoft.AspNetCore.Identity;

namespace IdSubjects.DependencyInjection;

/// <summary>
///     Options for IdSubjects.
/// </summary>
public class IdSubjectsOptions : IdentityOptions
{
    /// <summary>
    ///     已替换，提供Password有关选项。
    /// </summary>
    public new IdSubjectsPasswordOptions Password { get; set; } = new();

    /// <summary>
    ///     提供事件相关配置。
    /// </summary>
    public AuditEventsOptions Events { get; set; } = new();
}