using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdSubjects.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class SignInManagerTest(ServiceProviderFixture serviceProvider)
{
    [Fact]
    public async Task DisabledUserSignInShouldFail()
    {
        //Create a test user.
        var user = new ApplicationUser("TestUser")
        {
            Enabled = false,
        };

        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        await userManager.CreateAsync(user);

        var signInManager = scope.ServiceProvider.GetRequiredService<ApplicationUserSignInManager<ApplicationUser>>();

        var result = await signInManager.CheckPasswordSignInAsync(user, "Pass123$", false);
        Assert.False(result.Succeeded);
        Assert.True(result.IsNotAllowed);
    }

    [Fact]
    public async Task SignInMustChangePasswordWhenPasswordLastSetNull()
    {
        //Create a test user.
        var user = new ApplicationUser("TestUser");

        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        await userManager.CreateAsync(user, "Pass123$", true);

        var signInManager = scope.ServiceProvider.GetRequiredService<ApplicationUserSignInManager<ApplicationUser>>();

        var result = await signInManager.CheckPasswordSignInAsync(user, "Pass123$", false);
        Assert.False(result.Succeeded);
        Assert.True(result.MustChangePassword());
    }
}
