using IDSubjects;
using IDSubjects.DependencyInjection;
using IDSubjects.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for IDSubjects service injection.
/// </summary>
public static class IDSubjectsServiceCollectionExtensions
{

    /// <summary>
    /// 向基础设施添加AlphaID自然人标识管理功能。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdentityBuilder AddIDSubjects(this IServiceCollection services, Action<IDSubjectsOptions>? setupAction = null)
    {
        // 由IDSubjects使用的服务。
        services.TryAddScoped<OrganizationManager>();
        services.TryAddScoped<OrganizationMemberManager>();
        services.TryAddScoped<OrganizationSearcher>();

        //添加基础标识
        var builder = services.AddIdentityCore<NaturalPerson>()
            .AddUserManager<NaturalPersonManager>()
            .AddUserValidator<PhoneNumberValidator>()
            .AddErrorDescriber<NaturalPersonIdentityErrorDescriber>()
            ;

        if(setupAction != null)
        {
            services.Configure<IDSubjectsOptions>(setupAction);
        }

        return builder;
    }


}
