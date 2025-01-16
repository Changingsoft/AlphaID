using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSubjects.DependencyInjection;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using IdSubjects.SecurityAuditing;
using Microsoft.Extensions.DependencyInjection;

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
    IdSubjectsBuilder idSubjects, 
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
    public IdSubjectsBuilder IdSubjects { get; } = idSubjects;

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
}
