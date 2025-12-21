using AlphaIdPlatform.Invitations;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using IdSubjects.SecurityAuditing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Organizational;

namespace AlphaIdPlatform.DependencyInjection;

/// <summary>
///    AlphaID平台构建器。
/// </summary>
/// <param name="services"></param>
/// <param name="idSubjects"></param>
/// <param name="directoryLogon"></param>
/// <param name="realNameBuilder"></param>
/// <param name="auditLogBuilder"></param>
/// <param name="organizationalServiceBuilder"></param>
public class AlphaIdPlatformBuilder(IServiceCollection services,
    IdentityBuilder idSubjects,
    DirectoryLogonBuilder directoryLogon,
    RealNameBuilder realNameBuilder,
    AuditLogBuilder auditLogBuilder,
    OrganizationalServiceBuilder organizationalServiceBuilder)
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
    /// 组织管理。
    /// </summary>
    public OrganizationalServiceBuilder Organizational { get; } = organizationalServiceBuilder;
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
