using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDSubjects.RealName;
public class RealNameBuilder
{
    IServiceCollection services;

    public RealNameBuilder(IServiceCollection services)
    {
        this.services = services;
    }

    public RealNameBuilder AddRealNameStore<T>() where T : class, IRealNameStore
    {
        this.services.TryAddScoped<IRealNameStore, T>();
        return this;
    }
}
