using AlphaIdPlatform.DependencyInjection;
using AlphaIdPlatform.Identity;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
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
        services.AddScoped<DirectoryServiceManager>();
        services.AddScoped<DirectoryAccountManager>();
        var idSubjectsBuilder = services.AddIdSubjects();
        var directoryLoginBuilder = idSubjectsBuilder.AddDirectoryLogin();
        var realnameBuilder = idSubjectsBuilder.AddRealName();

        services.AddScoped<NaturalPersonService>();

        return new AlphaIdPlatformBuilder(services, idSubjectsBuilder, directoryLoginBuilder, realnameBuilder);
    }
}
