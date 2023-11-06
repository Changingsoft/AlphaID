using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDSubjects.RealName;
/// <summary>
/// 
/// </summary>
public class RealNameBuilder
{
    private readonly IServiceCollection services;

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
