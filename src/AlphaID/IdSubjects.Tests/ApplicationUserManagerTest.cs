using IntegrationTestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdSubjects.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class ApplicationUserManagerTest(ServiceProviderFixture serviceProvider)
{
    [Fact]
    public async Task SetTimeZone()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user);

        //using IANA time zone name
        IdentityResult result = await manager.SetTimeZone(user, "Asia/Shanghai");
        Assert.True(result.Succeeded);
        Assert.Equal("Asia/Shanghai", user.TimeZone);

        //using Microsoft time zone name
        result = await manager.SetTimeZone(user, "China Standard Time");
        Assert.True(result.Succeeded);
        Assert.Equal("Asia/Shanghai", user.TimeZone);
    }

    [Fact]
    public async Task CreateWithPassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();

        DateTimeOffset now = DateTimeOffset.UtcNow;
        manager.TimeProvider = new FrozenTimeProvider(now);
        var user = new ApplicationUser("TestUser");
        IdentityResult result = await manager.CreateAsync(user, "Pass123$");

        Assert.True(result.Succeeded);
        Assert.NotNull(user.PasswordHash);
        Assert.Equal(now, user.PasswordLastSet!.Value);
        Assert.Equal(now, user.WhenCreated);
        Assert.Equal(now, user.WhenChanged);
    }

    [Fact]
    public async Task CreateWithoutPassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();

        manager.TimeProvider = new FrozenTimeProvider();

        var user = new ApplicationUser("TestUser");

        IdentityResult result = await manager.CreateAsync(user);

        Assert.True(result.Succeeded);
        Assert.False(user.PasswordLastSet.HasValue);
        Assert.Equal(manager.TimeProvider.GetUtcNow(), user.WhenCreated);
        Assert.Equal(manager.TimeProvider.GetUtcNow(), user.WhenChanged);
    }

    [Fact]
    public async Task ResetPasswordForNoPasswordUser()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();

        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user);

        //Reset password for user without password
        var newPassword = "Pass123$";
        await manager.ResetPasswordAsync(user, newPassword, true, true);

        Assert.NotNull(user.PasswordHash);
        Assert.Null(user.PasswordLastSet);
    }

    [Fact]
    public async Task UserResetPasswordWithOldPassword()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        manager.TimeProvider = new FrozenTimeProvider();
        manager.Options.Tokens.PasswordResetTokenProvider = "Static";
        manager.RegisterTokenProvider("Static", new StaticTokenProvider());

        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user, "Pass123$"); //1st password
        await manager.ChangePasswordAsync(user, "Pass123$", "NewPass123$"); //2nd password

        //Reset password with old password
        var token = await manager.GeneratePasswordResetTokenAsync(user);
        var newPassword = "Pass123$"; //reuse 1st password
        var result = await manager.ResetPasswordAsync(user, token, newPassword);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task SetUpdateTimeWhenUpdate()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();

        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user);

        var utcNow = new DateTimeOffset(2023, 11, 4, 3, 50, 34, TimeSpan.Zero);
        manager.TimeProvider = new FrozenTimeProvider(utcNow);

        await manager.UpdateAsync(user);

        Assert.Equal(utcNow, user.WhenChanged);
    }

    [Fact]
    public async Task AddPasswordWillSetPasswordLastSetTime()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user);

        IdentityResult result = await manager.AddPasswordAsync(user, "Password$1");
        Assert.True(result.Succeeded);
        Assert.NotNull(user.PasswordLastSet);
    }

    [Fact]
    public async Task UnlockUser()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user, "Pass123$");
        //调用AccessFailedAsync 5次以锁定用户。
        for(int i = 0; i < 5; i++)
        {
            await manager.AccessFailedAsync(user);
        }
        Assert.True(await manager.IsLockedOutAsync(user));

        await manager.UnlockUserAsync(user);
        Assert.Null(user.LockoutEnd);
        Assert.False(await manager.IsLockedOutAsync(user));
    }

    [Fact]
    public async Task RemovePassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user, "Pass123$");

        IdentityResult result = await manager.RemovePasswordAsync(user);
        Assert.True(result.Succeeded);
        Assert.Null(user.PasswordLastSet);
    }

    [Fact]
    public async Task ChangePassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        manager.PasswordLifetime.RememberPasswordHistory = 1;
        var user = new ApplicationUser("TestUser");
        await manager.CreateAsync(user, "Pass123$");

        IdentityResult result = await manager.ChangePasswordAsync(user, "Pass123$", "Pass1234$");
        Assert.True(result.Succeeded);

        //change password again with same old password will fail.
        result = await manager.ChangePasswordAsync(user, "Pass1234$", "Pass123$");
        Assert.False(result.Succeeded);
    }
}