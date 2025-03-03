using AlphaIdPlatform.DependencyInjection;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.Subjects;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
// ReSharper restore CheckNamespace

/// <summary>
/// Extension methods for setting up AlphaIdPlatform services in an <see cref="IServiceCollection" />.
/// </summary>
public static class AlphaIdPlatformServiceCollectionExtensions
{
    /// <summary>
    /// 添加AlphaId平台服务。
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static AlphaIdPlatformBuilder AddAlphaIdPlatform(this IServiceCollection services)
    {
        //IdSubjects
        var idSubjectsBuilder = services.AddIdSubjects<NaturalPerson>();
        idSubjectsBuilder.AddProfileUrlGenerator<NaturalPersonProfileGenerator, NaturalPerson>();

        //DirectoryService
        var directoryLoginBuilder = idSubjectsBuilder.AddDirectoryLogin<NaturalPerson>();

        //RealName
        var realnameBuilder = idSubjectsBuilder.AddRealName<NaturalPerson>();

        //AuditLog
        var auditLogBuilder = services.AddAuditLog();

        //平台服务。
        services.TryAddScoped<OrganizationManager>();
        services.TryAddScoped<OrganizationSearcher>();
        services.TryAddScoped<OrganizationMemberManager>();
        services.TryAddScoped<JoinOrganizationInvitationManager>();
        services.AddScoped<NaturalPersonService>();

        return new AlphaIdPlatformBuilder(services, idSubjectsBuilder, directoryLoginBuilder, realnameBuilder, auditLogBuilder);
    }
}
