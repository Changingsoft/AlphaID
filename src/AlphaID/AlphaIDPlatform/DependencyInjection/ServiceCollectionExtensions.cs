using AlphaIdPlatform.DependencyInjection;
using AlphaIdPlatform.Identity;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;

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
        var idSubjectsBuilder = services.AddIdSubjects();
        var directoryLoginBuilder = idSubjectsBuilder.AddDirectoryLogin();
        var realnameBuilder = idSubjectsBuilder.AddRealName();
        var auditLogBuilder = services.AddAuditLog();

        services.AddScoped<NaturalPersonService>();

        return new AlphaIdPlatformBuilder(services, idSubjectsBuilder, directoryLoginBuilder, realnameBuilder, auditLogBuilder);
    }
}
