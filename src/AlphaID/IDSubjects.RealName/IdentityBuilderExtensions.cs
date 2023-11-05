using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
