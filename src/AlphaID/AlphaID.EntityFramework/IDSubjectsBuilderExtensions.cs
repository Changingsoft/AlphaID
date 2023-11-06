using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlphaID.EntityFramework;
public static class IDSubjectsBuilderExtensions
{
    public static IdentityBuilder AddDefaultStores(this IdentityBuilder builder)
    {
        builder.Services.TryAddScoped<IOrganizationStore, OrganizationStore>();
        builder.Services.TryAddScoped<IOrganizationMemberStore, OrganizationMemberStore>();
        builder.AddUserStore<NaturalPersonStore>();
        builder.Services.AddScoped<IQueryableOrganizationUsedNameStore, OrganizationUsedNameStore>();
        return builder;
    }
}
