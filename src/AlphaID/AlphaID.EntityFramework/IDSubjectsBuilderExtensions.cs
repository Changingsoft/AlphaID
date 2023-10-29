using IDSubjects;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaID.EntityFramework;
public static class IDSubjectsBuilderExtensions
{
    public static IDSubjectsBuilder AddDefaultStores(this IDSubjectsBuilder builder)
    {
        builder.AddOrganizationStore<OrganizationStore>();
        builder.AddPersonStore<NaturalPersonStore>();
        builder.AddOrganizationMemberStore<OrganizationMemberStore>();
        builder.Services.AddScoped<IQueryableOrganizationUsedNameStore, OrganizationUsedNameStore>();
        return builder;
    }
}
