using Microsoft.AspNetCore.Cors.Infrastructure;

namespace AlphaIdWebAPI;

internal static class CorsPolicyExtensions
{
    private const string WildcardSubdomain = "*.";

    public static bool IsOriginAllowedSubdomainAndLocalhost(this CorsPolicy policy, string origin)
    {
        return policy.Origins.Contains(origin)
               || (Uri.TryCreate(origin, UriKind.Absolute, out Uri? originUri)
                   && (originUri.IsLoopback
                       || policy.Origins
                           .Where(o => o.Contains($"://{WildcardSubdomain}"))
                           .Select(CreateDomainUri)
                           .Any(domain => UriHelpers.IsSubdomainOf(originUri, domain))));
    }

    private static Uri CreateDomainUri(string origin)
    {
        return new Uri(origin.Replace(WildcardSubdomain, string.Empty), UriKind.Absolute);
    }
}