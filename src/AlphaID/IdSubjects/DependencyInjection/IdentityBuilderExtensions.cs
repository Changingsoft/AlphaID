using IdSubjects;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection;
public static class IdentityBuilderExtensions
{
    public static IdentityBuilder AddProfileUrlGenerator<TGenerator, TUser>(this IdentityBuilder builder)
        where TGenerator : ProfileUrlGenerator<TUser>
        where TUser : ApplicationUser
    {
        builder.Services.AddScoped<ProfileUrlGenerator<TUser>, TGenerator>();
        return builder;
    }

    public static IdentityBuilder AddPasswordHistoryStore<T>(this IdentityBuilder builder)
        where T : class, IPasswordHistoryStore
    {
        builder.Services.AddScoped<IPasswordHistoryStore, T>();
        return builder;
    }
}
