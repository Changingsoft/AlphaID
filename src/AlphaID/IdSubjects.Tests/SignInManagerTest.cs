using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdSubjects.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class SignInManagerTest(ServiceProviderFixture serviceProvider)
{
    [Fact]
    public async Task DisabledUserSignInShouldFail()
    {
        var user = new ApplicationUser()
        {
            Enabled = false,
        };

        var userManager = serviceProvider.RootServiceProvider
            .GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        await userManager.CreateAsync(user);

        var signInManager = serviceProvider.RootServiceProvider
            .GetRequiredService<ApplicationUserSignInManager<ApplicationUser>>();

        var result = await signInManager.CheckPasswordSignInAsync(user, "Pass123$", false);
        Assert.False(result.Succeeded);
        Assert.True(result.IsNotAllowed);
    }
}
