using IdSubjects.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// </summary>
public static class IdSubjectsBuilderExtensions
{
    /// <summary>
    ///     向基础结构添加Directory目录管理功能。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>返回目录管理构造器。</returns>
    public static DirectoryLogonBuilder AddDirectoryLogin(this IdSubjectsBuilder builder)
    {
        builder.Services.TryAddScoped<DirectoryServiceManager>();
        builder.Services.TryAddScoped<DirectoryAccountManager>();

        //注册所需拦截器。
        builder.AddInterceptor<DirectoryAccountUpdateInterceptor>();
        builder.AddInterceptor<DirectoryAccountCreateInterceptor>();
        builder.AddInterceptor<UserPasswordInterceptor>();

        builder.Services.AddScoped<ISubjectGenerator, AdfsSubjectGenerator>();

        var directoryLogonBuilder = new DirectoryLogonBuilder(builder.Services);
        return directoryLogonBuilder;
    }
}