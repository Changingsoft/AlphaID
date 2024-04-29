using IdSubjects;
using IdSubjects.DependencyInjection;
using IdSubjects.Invitations;
using IdSubjects.Payments;
using IdSubjects.SecurityAuditing;
using IdSubjects.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Extensions for IdSubjects service injection.
/// </summary>
public static class IdSubjectsServiceCollectionExtensions
{
    /// <summary>
    ///     向基础设施添加AlphaId自然人标识管理功能。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdSubjectsBuilder AddIdSubjects(this IServiceCollection services,
        Action<IdSubjectsOptions>? setupAction = null)
    {
        //IdSubjects需要访问HttpContext.
        services.AddHttpContextAccessor();

        // 由IdSubjects使用的服务。
        services.TryAddScoped<NaturalPersonManager>();
        services.TryAddScoped<OrganizationManager>();
        services.TryAddScoped<OrganizationMemberManager>();
        services.TryAddScoped<OrganizationSearcher>();
        services.TryAddScoped<NaturalPersonIdentityErrorDescriber>();
        services.TryAddScoped<PersonBankAccountManager>();
        services.TryAddScoped<JoinOrganizationInvitationManager>();
        services.TryAddScoped<OrganizationBankAccountManager>();
        services.TryAddScoped<OrganizationIdentifierManager>();
        services.TryAddScoped<OrganizationIdentifierValidator, UsccValidator>();
        services.TryAddScoped<PasswordHistoryManager>();

        services.TryAddScoped<IEventService, DefaultEventService>();
        services.TryAddScoped<IEventSink, DefaultEventSink>();

        services.AddScoped<ISubjectGenerator, DefaultSubjectGenerator>();

        //添加基础标识
        IdentityBuilder identityBuilder = services.AddIdentityCore<NaturalPerson>()
                .AddUserManager<NaturalPersonManager>() //as UserManager<NaturalPerson>
                .AddUserValidator<PhoneNumberValidator>()
            ;

        if (setupAction != null) services.Configure(setupAction);

        return new IdSubjectsBuilder(services, identityBuilder);
    }
}