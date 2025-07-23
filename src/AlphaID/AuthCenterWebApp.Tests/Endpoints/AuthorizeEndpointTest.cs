using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace AuthCenterWebApp.Tests.Endpoints;

public class AuthorizeEndpointTest
{
    [Fact]
    public async Task RedirectForAuthentication()
    {
        var factory = new AuthCenterWebAppFactory();
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // 1. 生成 code_verifier
        var codeVerifier = GenerateCodeVerifier();

        // 2. 生成 code_challenge
        var codeChallenge = GenerateCodeChallenge(codeVerifier);


        var queryParams = new Dictionary<string, string?>
        {
            { "client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c" },
            { "response_type", "code" },
            { "response_mode", "form_post" },
            { "scope", "openid profile" },
            { "redirect_uri", "https://localhost:49728/signin-oidc" },
            { "code_challenge", codeChallenge },
            { "code_challenge_method", "S256" },
            { "nonce", "nonce" },
            { "state", "state" }
        };

        string urlWithQuery = QueryHelpers.AddQueryString("/connect/authorize", queryParams);

        HttpResponseMessage response = await client.GetAsync(urlWithQuery);
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        var location = response.Headers.Location;
        var isOptions = factory.Services.GetRequiredService<IOptions<IdentityServerOptions>>();
        Assert.Equal(isOptions.Value.UserInteraction.LoginUrl, location!.AbsolutePath);
    }

    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        var bytes = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}