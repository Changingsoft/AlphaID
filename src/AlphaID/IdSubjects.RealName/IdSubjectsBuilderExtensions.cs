using IdSubjects.DependencyInjection;
using IdSubjects.RealName.Requesting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.RealName;

/// <summary>
/// IdentityBuilder的扩展。
/// </summary>
public static class IdSubjectsBuilderExtensions
{
    /// <summary>
    /// 添加实名认证功能。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>返回一个实名认证构造器，见<see cref="RealNameBuilder"/>。</returns>
    public static RealNameBuilder AddRealName(this IdSubjectsBuilder builder)
    {
        //Add services
        builder.Services.TryAddScoped<RealNameManager>();
        builder.Services.TryAddScoped<RealNameRequestManager>();

        //添加拦截器。
        builder.AddInterceptor<RealNameInterceptor>();

        var realNameBuilder = new RealNameBuilder(builder.Services);
        return realNameBuilder;
    }
}
