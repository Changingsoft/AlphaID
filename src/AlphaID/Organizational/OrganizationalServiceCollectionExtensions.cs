using Microsoft.Extensions.DependencyInjection.Extensions;
using Organizational;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
public static class OrganizationalServiceCollectionExtensions
{
    public static OrganizationalServiceBuilder AddOrganizational(this IServiceCollection services)
    {
        services.TryAddScoped<OrganizationManager>();
        services.AddScoped<OrganizationMemberManager>();
        return new OrganizationalServiceBuilder(services);
    }
}
