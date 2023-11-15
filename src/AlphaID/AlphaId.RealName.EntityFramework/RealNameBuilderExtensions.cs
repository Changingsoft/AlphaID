using IdSubjects.RealName;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaId.RealName.EntityFramework;
public static class RealNameBuilderExtensions
{
    /// <summary>
    /// 添加必要的实名认证相关存取器。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static RealNameBuilder AddDefaultStores(this RealNameBuilder builder)
    {
        builder.AddRealNameStore<RealNameStateStore>();
        return builder;
    }

    /// <summary>
    /// 向基础结构添加实名认证所需DbContext.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="optionsBuilder"></param>
    /// <returns></returns>
    public static RealNameBuilder AddDbContext(this RealNameBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder)
    {
        builder.Services.AddDbContext<RealNameDbContext>(optionsBuilder);
        return builder;
    }
}
