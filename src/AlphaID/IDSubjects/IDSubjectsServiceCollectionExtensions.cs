using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDSubjects;

/// <summary>
/// Extensions for IDSubjects service injection.
/// </summary>
public static class IDSubjectsServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IDSubjectsBuilder AddIDSubjects(this IServiceCollection services)
    {
        services.TryAddScoped<NaturalPersonManager>();
        services.TryAddScoped<OrganizationManager>();
        services.TryAddScoped<OrganizationMemberManager>();
        services.TryAddScoped<OrganizationSearcher>();
        var builder = new IDSubjectsBuilder(services);
        return builder;
    }
}
