using IntegrationTestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdSubjects.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class ApplicationUserManagerTest(ServiceProviderFixture serviceProvider)
{
    private readonly ApplicationUser _person = new("zhangsan");

    [Fact]
    public async Task SetTimeZone()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        await manager.CreateAsync(_person);

        //using IANA time zone name
        IdentityResult result = await manager.SetTimeZone(_person, "Asia/Shanghai");
        Assert.True(result.Succeeded);
        Assert.Equal("Asia/Shanghai", _person.TimeZone);

        //using Microsoft time zone name
        result = await manager.SetTimeZone(_person, "China Standard Time");
        Assert.True(result.Succeeded);
        Assert.Equal("Asia/Shanghai", _person.TimeZone);
    }

    [Fact]
    public async Task CreateWithPassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();

        DateTimeOffset now = DateTimeOffset.UtcNow;
        manager.TimeProvider = new FrozenTimeProvider(now);
        IdentityResult result = await manager.CreateAsync(_person, "Pass123$");

        Assert.True(result.Succeeded);
        Assert.NotNull(_person.PasswordHash);
        Assert.Equal(now, _person.PasswordLastSet!.Value);
        Assert.Equal(now, _person.WhenCreated);
        Assert.Equal(now, _person.WhenChanged);
    }

    [Fact]
    public async Task CreateWithoutPassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();

        DateTimeOffset now = DateTimeOffset.UtcNow;
        manager.TimeProvider = new FrozenTimeProvider(now);

        IdentityResult result = await manager.CreateAsync(_person);

        Assert.True(result.Succeeded);
        Assert.False(_person.PasswordLastSet.HasValue);
        Assert.Equal(now, _person.WhenCreated);
        Assert.Equal(now, _person.WhenChanged);
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
        await manager.CreateAsync(_person);

        var utcNow = new DateTimeOffset(2023, 11, 4, 3, 50, 34, TimeSpan.Zero);
        manager.TimeProvider = new FrozenTimeProvider(utcNow);

        await manager.UpdateAsync(_person);

        Assert.Equal(utcNow, _person.WhenChanged);
    }

    [Fact]
    public async Task AddPasswordWillSetPasswordLastSetTime()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await manager.CreateAsync(_person);

        IdentityResult result = await manager.AddPasswordAsync(_person, "Password$1");
        Assert.True(result.Succeeded);
        Assert.NotNull(_person.PasswordLastSet);
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
        Assert.Null(_person.LockoutEnd);
        Assert.False(await manager.IsLockedOutAsync(user));
    }

    [Fact]
    public async Task RemovePassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await manager.CreateAsync(_person, "Pass123$");

        IdentityResult result = await manager.RemovePasswordAsync(_person);
        Assert.True(result.Succeeded);
        Assert.Null(_person.PasswordLastSet);
    }

    [Fact]
    public async Task ChangePassword()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        manager.PasswordLifetime.RememberPasswordHistory = 1;

        await manager.CreateAsync(_person, "Pass123$");

        IdentityResult result = await manager.ChangePasswordAsync(_person, "Pass123$", "Pass1234$");
        Assert.True(result.Succeeded);

        var passwordHistoryStore = scope.ServiceProvider.GetRequiredService<IPasswordHistoryStore>();
        IEnumerable<string> passwords = passwordHistoryStore.GetPasswords(_person.Id, 10);
        Assert.Single(passwords);

        //change password again with same old password will fail.
        result = await manager.ChangePasswordAsync(_person, "Pass1234$", "Pass123$");
        Assert.False(result.Succeeded);
    }
}