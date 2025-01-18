using AlphaIdPlatform.Identity;
using IdSubjects;
using IdSubjects.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaId.EntityFramework.IdSubjects;

public static class IdSubjectsBuilderExtensions
{
    /// <summary>
    ///     向AspNetCore Identity基础结构添加默认的存取器实现。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IdSubjectsBuilder AddDefaultStores(this IdSubjectsBuilder builder)
    {
        builder.AddPersonStore<ApplicationUserStore, NaturalPerson>();
        builder.AddPasswordHistoryStore<PasswordHistoryStore>();

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