using AlphaIdPlatform.DependencyInjection;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.Payments;
using IdSubjects;
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
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AlphaIdPlatform services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static AlphaIdPlatformBuilder AddAlphaIdPlatform(this IServiceCollection services)
    {
        //Add required services
        var idSubjectsBuilder = services.AddIdSubjects<NaturalPerson>();
        var directoryLoginBuilder = idSubjectsBuilder.AddDirectoryLogin<NaturalPerson>();
        var realnameBuilder = idSubjectsBuilder.AddRealName<NaturalPerson>();
        var auditLogBuilder = services.AddAuditLog();

        services.TryAddScoped<OrganizationMemberManager>();
        services.TryAddScoped<JoinOrganizationInvitationManager>();
        services.TryAddScoped<ApplicationUserBankAccountManager>();

        services.AddScoped<NaturalPersonService>();

        return new AlphaIdPlatformBuilder(services, idSubjectsBuilder, directoryLoginBuilder, realnameBuilder, auditLogBuilder);
    }
}
