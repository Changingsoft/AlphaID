using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;
public static class IdentityBuilderExtensions
{
    public static IdentityBuilder AddProfileUrlGenerator<TGenerator, TUser>(this IdentityBuilder builder)
        where TGenerator : ProfileUrlGenerator<TUser>
        where TUser : ApplicationUser
    {
        builder.Services.TryAddScoped<ProfileUrlGenerator<TUser>, TGenerator>();
        return builder;
    }

    public static IdentityBuilder AddPasswordHistoryStore<T>(this IdentityBuilder builder)
        where T : class, IPasswordHistoryStore
    {
        builder.Services.TryAddScoped<IPasswordHistoryStore, T>();
        return builder;
    }
}
