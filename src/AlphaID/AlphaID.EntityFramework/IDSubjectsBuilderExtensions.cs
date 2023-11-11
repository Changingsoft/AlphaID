using IDSubjects;
using IDSubjects.Invitations;
using IDSubjects.Payments;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlphaID.EntityFramework;
public static class IdSubjectsBuilderExtensions
{
    public static IdentityBuilder AddDefaultStores(this IdentityBuilder builder)
    {
        builder.Services.TryAddScoped<IOrganizationStore, OrganizationStore>();
        builder.Services.TryAddScoped<IOrganizationMemberStore, OrganizationMemberStore>();
        builder.AddUserStore<NaturalPersonStore>();
        builder.Services.TryAddScoped<IQueryableOrganizationUsedNameStore, OrganizationUsedNameStore>();
        builder.Services.TryAddScoped<IPersonBankAccountStore, PersonBankAccountStore>();
        builder.Services.TryAddScoped<IJoinOrganizationInvitationStore, JoinOrganizationInvitationStore>();
        builder.Services.TryAddScoped<IOrganizationBankAccountStore, OrganizationBankAccountStore>();

        return builder;
    }
}
