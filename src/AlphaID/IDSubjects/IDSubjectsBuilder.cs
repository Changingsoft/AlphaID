using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDSubjects;
public class IDSubjectsBuilder
{
    public IServiceCollection Services { get; }

    public IDSubjectsBuilder(IServiceCollection services)
    {
        this.Services = services;
    }

    public IDSubjectsBuilder AddPersonStore<T>() where T : IUserStore<NaturalPerson>
    {
        this.Services.TryAddScoped(typeof(IUserStore<NaturalPerson>), typeof(T));
        return this;
    }

    public IDSubjectsBuilder AddOrganizationStore<T>() where T : IOrganizationStore
    {
        this.Services.TryAddScoped(typeof(IOrganizationStore), typeof(T));
        return this;
    }

    public IDSubjectsBuilder AddOrganizationMemberStore<T>() where T : IOrganizationMemberStore
    {
        this.Services.TryAddScoped(typeof(IOrganizationMemberStore), typeof(T));
        return this;
    }
}
