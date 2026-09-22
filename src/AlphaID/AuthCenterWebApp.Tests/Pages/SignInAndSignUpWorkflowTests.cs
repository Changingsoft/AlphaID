using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace AuthCenterWebApp.Tests.Pages;

[Collection<TestServerCollection>]
public class SignInAndSignUpWorkflowTests(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task UserSignInOrSignUpWhenProvideVerificationCodeService()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false,
        });
        var response = await client.GetAsync("/Settings/Profile", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal("/Account/SignInOrSignUp", response.Headers.Location!.AbsolutePath, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UseWeixinMpAsExternalLogin()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false,
        });
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MicroMessenger");

        // Step 1: Access protected resource, then redirect to SignInOrSignUp page.
        var response = await client.GetAsync("/Settings/Profile", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        var location = response.Headers.Location!;

        // Step 2: 满足条件，将重定向到外部登录锚点
        response = await client.GetAsync(location, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        location = response.Headers.Location!;

        // Step 3: 出发重定向到微信
        response = await client.GetAsync(location, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);

        Assert.Equal("open.weixin.qq.com", response.Headers.Location!.Host, StringComparer.OrdinalIgnoreCase);
        Assert.Equal("/connect/oauth2/authorize", response.Headers.Location!.AbsolutePath, StringComparer.OrdinalIgnoreCase);

    }
}
