using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDSubjects.RealName;

/// <summary>
/// IdentityBuilder的扩展。
/// </summary>
public static class IdentityBuilderExtensions
{
    /// <summary>
    /// 添加实名认证功能。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static RealNameBuilder AddRealName(this IdentityBuilder builder)
    {
        //Add required services
        builder.Services.TryAddScoped<RealNameManager>();

        var realNameBuilder = new RealNameBuilder(builder.Services);
        return realNameBuilder;
    }
}
