using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.Subjects;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using IdSubjects.SecurityAuditing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlphaIdPlatform.DependencyInjection;

/// <summary>
///    AlphaID平台构建器。
/// </summary>
/// <param name="services"></param>
/// <param name="idSubjects"></param>
/// <param name="directoryLogon"></param>
/// <param name="realNameBuilder"></param>
/// <param name="auditLogBuilder"></param>
public class AlphaIdPlatformBuilder(IServiceCollection services,
    IdentityBuilder idSubjects,
    DirectoryLogonBuilder directoryLogon,
    RealNameBuilder realNameBuilder,
    AuditLogBuilder auditLogBuilder)
{
    /// <summary>
    ///    获取服务集合。
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    ///   获取身份主体构建器。
    /// </summary>
    public IdentityBuilder IdSubjects { get; } = idSubjects;

    /// <summary>
    ///  获取目录登录构建器。
    /// </summary>
    public DirectoryLogonBuilder DirectoryLogon { get; } = directoryLogon;

    /// <summary>
    /// 获取实名认证构建器。
    /// </summary>
    public RealNameBuilder RealName { get; } = realNameBuilder;

    /// <summary>
    /// 获取审计日志构建器。
    /// </summary>
    public AuditLogBuilder AuditLog { get; } = auditLogBuilder;

    /// <summary>
    /// Add generic organization store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AlphaIdPlatformBuilder AddOrganizationStore<T>() where T : class, IOrganizationStore
    {
        Services.TryAddScoped<IOrganizationStore, T>();
        return this;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AlphaIdPlatformBuilder AddJoinOrganizationInvitationStore<T>() where T : class, IJoinOrganizationInvitationStore
    {
        Services.TryAddScoped<IJoinOrganizationInvitationStore, T>();
        return this;
    }

}
