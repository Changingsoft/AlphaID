using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organizational;
public class OrganizationalServiceBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;

    /// <summary>
    /// Add generic organization store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public OrganizationalServiceBuilder AddOrganizationStore<T>() where T : class, IOrganizationStore
    {
        Services.TryAddScoped<IOrganizationStore, T>();
        return this;
    }

}
