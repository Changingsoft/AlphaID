using IdSubjects.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaId.EntityFramework;

public static class IdSubjectsBuilderExtensions
{
    /// <summary>
    ///     向AspNetCore Identity基础结构添加默认的存取器实现。
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

    /// <summary>
    ///     添加适用于IdSubjects的DbContext。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IdSubjectsBuilder AddDbContext(this IdSubjectsBuilder builder,
        Action<DbContextOptionsBuilder> options)
    {
        builder.Services.AddDbContext<IdSubjectsDbContext>(options);
        return builder;
    }
}