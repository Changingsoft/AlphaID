using IdSubjects.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaIdPlatform.Identity;

/// <summary>
/// </summary>
public class IdSubjectsIdentityBuilder
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="idSubjectsBuilder"></param>
    /// <param name="authenticationBuilder"></param>
    public IdSubjectsIdentityBuilder(IServiceCollection services,
        IdSubjectsBuilder idSubjectsBuilder,
        AuthenticationBuilder authenticationBuilder)
    {
        IdSubjects = idSubjectsBuilder;
        Authentication = authenticationBuilder;
        Services = services;
    }

    /// <summary>
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// </summary>
    public IdSubjectsBuilder IdSubjects { get; }

    /// <summary>
    /// </summary>
    public AuthenticationBuilder Authentication { get; }
}