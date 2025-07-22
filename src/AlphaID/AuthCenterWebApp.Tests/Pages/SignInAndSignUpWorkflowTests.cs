using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AlphaIdPlatform.Platform;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCenterWebApp.Tests.Pages;
public class SignInAndSignUpWorkflowTests
{
    [Fact]
    public async Task UserSignInOrSignUpWhenProvideVerificationCodeService()
    {
        var factory = new AuthCenterWebAppFactory();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false,
        });
        var response = await client.GetAsync("/Settings/Profile");
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal("/Account/SignInOrSignUp", response.Headers.Location!.AbsolutePath, StringComparer.OrdinalIgnoreCase);
    }
}
