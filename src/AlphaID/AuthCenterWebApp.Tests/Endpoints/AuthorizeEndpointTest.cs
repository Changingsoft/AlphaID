using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace AuthCenterWebApp.Tests.Endpoints;

[Collection(nameof(TestServerCollection))]
public class AuthorizeEndpointTest(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task GotoAuthenticationPage()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false,
        });

        var response = await client.GetAsync("/connect/authorize");
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
    }
}
