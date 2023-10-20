using IDSubjects;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for IDSubjects service injection.
/// </summary>
public static class IDSubjectsServiceCollectionExtensions
{
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
