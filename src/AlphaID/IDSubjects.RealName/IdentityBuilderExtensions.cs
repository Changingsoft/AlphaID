using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDSubjects.RealName;
public static class IdentityBuilderExtensions
{
    public static RealNameBuilder AddRealName(this IdentityBuilder builder)
    {
        //Add required services
        builder.Services.TryAddScoped<RealNameManager>();

        var realNameBuilder = new RealNameBuilder(builder.Services);
        return realNameBuilder;
    }
}
