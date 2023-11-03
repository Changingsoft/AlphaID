using IDSubjects;
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
    public static IdentityBuilder AddIdSubjectsIdentityCore(this IServiceCollection services, Action<IdentityOptions> setupAction)
    {
        // 由IDSubjects使用的服务。
        services.TryAddScoped<OrganizationManager>();
        services.TryAddScoped<OrganizationMemberManager>();
        services.TryAddScoped<OrganizationSearcher>();

        //添加基础标识
        var builder = services.AddIdentityCore<NaturalPerson>(setupAction)
            .AddUserManager<NaturalPersonManager>()
            //.AddUserValidator<PhoneNumberValidator>()
            //.AddErrorDescriber<NaturalPersonIdentityErrorDescriber>()
            ;
        return builder;
    }


}
