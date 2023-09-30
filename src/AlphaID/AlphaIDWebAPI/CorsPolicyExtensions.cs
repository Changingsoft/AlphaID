using Microsoft.AspNetCore.Cors.Infrastructure;

namespace AlphaIDWebAPI;

internal static class CorsPolicyExtensions
{
    private const string _WildcardSubdomain = "*.";

    public static bool IsOriginAllowedSubdomainAndLocalhost(this CorsPolicy policy, string origin)
    {
        return policy.Origins.Contains(origin)
            || Uri.TryCreate(origin, UriKind.Absolute, out var originUri)
            && (originUri.IsLoopback
            || policy.Origins
                .Where(o => o.Contains($"://{_WildcardSubdomain}"))
                .Select(CreateDomainUri)
                .Any(domain => UriHelpers.IsSubdomainOf(originUri, domain)));
    }

    private static Uri CreateDomainUri(string origin)
    {
        return new Uri(origin.Replace(_WildcardSubdomain, string.Empty), UriKind.Absolute);
    }
}
