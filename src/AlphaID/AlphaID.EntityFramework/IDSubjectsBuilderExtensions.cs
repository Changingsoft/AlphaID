using IdSubjects;
using IdSubjects.Invitations;
using IdSubjects.Payments;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlphaId.EntityFramework;
public static class IdSubjectsBuilderExtensions
{
    /// <summary>
    /// 向AspNetCore Identity基础结构添加默认的存取器实现。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IdentityBuilder AddDefaultStores(this IdentityBuilder builder)
    {
        builder.Services.TryAddScoped<IOrganizationStore, OrganizationStore>();
        builder.Services.TryAddScoped<IOrganizationMemberStore, OrganizationMemberStore>();
        builder.AddUserStore<NaturalPersonStore>();
        builder.Services.TryAddScoped<IPersonBankAccountStore, PersonBankAccountStore>();
        builder.Services.TryAddScoped<IJoinOrganizationInvitationStore, JoinOrganizationInvitationStore>();
        builder.Services.TryAddScoped<IOrganizationBankAccountStore, OrganizationBankAccountStore>();
        builder.Services.TryAddScoped<IOrganizationIdentifierStore, OrganizationIdentifierStore>();

        return builder;
    }
}
