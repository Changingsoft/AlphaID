using IDSubjects;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

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
        services.TryAddScoped<IDSubjectsErrorDescriber>();
        var builder = new IDSubjectsBuilder(services);
        return builder;
    }
}
