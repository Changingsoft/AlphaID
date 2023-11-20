using IdSubjects;
using IdSubjects.DependencyInjection;
using IdSubjects.Invitations;
using IdSubjects.Payments;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.CompilerServices;

namespace AlphaId.EntityFramework;
public static class IdSubjectsBuilderExtensions
{
    /// <summary>
    /// 向AspNetCore Identity基础结构添加默认的存取器实现。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IdSubjectsBuilder AddDefaultStores(this IdSubjectsBuilder builder)
    {
        builder.AddPersonStore<NaturalPersonStore>();
        builder.AddOrganizationStore<OrganizationStore>();
        builder.AddOrganizationMemberStore<OrganizationMemberStore>();
        builder.AddPasswordHistoryStore<PasswordHistoryStore>();
        builder.AddPersonBankAccountStore<PersonBankAccountStore>();
        builder.AddJoinOrganizationInvitationStore<JoinOrganizationInvitationStore>();
        builder.AddOrganizationBankAccountStore<OrganizationBankAccountStore>();
        builder.AddOrganizationIdentifierStore<OrganizationIdentifierStore>();

        return builder;
    }

    public static IdSubjectsBuilder AddDbContext(this IdSubjectsBuilder builder,
        Action<DbContextOptionsBuilder> options)
    {
        builder.Services.AddDbContext<IdSubjectsDbContext>(options);
        return builder;
    }
}
