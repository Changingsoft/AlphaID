using IntegrationTestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdSubjects.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class NaturalPersonManagerTest(ServiceProviderFixture serviceProvider)
{
    private readonly NaturalPerson person = new("zhangsan", new PersonNameInfo("张三"));

    [Fact]
    public async Task SetTimeZone()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        await manager.CreateAsync(this.person);

        //using IANA time zone name
        var result = await manager.SetTimeZone(this.person, "Asia/Shanghai");
        Assert.True(result.Succeeded);
        Assert.Equal("Asia/Shanghai", person.TimeZone);

        //using Microsoft time zone name
        result = await manager.SetTimeZone(this.person, "China Standard Time");
        Assert.True(result.Succeeded);
        Assert.Equal("Asia/Shanghai", person.TimeZone);
    }

    [Fact]
    public async Task CreateWithPassword()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();

        var now = DateTimeOffset.UtcNow;
        manager.TimeProvider = new FrozenTimeProvider(now);
        var result = await manager.CreateAsync(person, "Pass123$");

        Assert.True(result.Succeeded);
        Assert.NotNull(person.PasswordHash);
        Assert.Equal(now, person.PasswordLastSet!.Value);
        Assert.Equal(now, this.person.WhenCreated);
        Assert.Equal(now, this.person.WhenChanged);
        Assert.Equal(now, this.person.PersonWhenChanged);
    }

    [Fact]
    public async Task CreateWithoutPassword()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        
        var now = DateTimeOffset.UtcNow;
        manager.TimeProvider = new FrozenTimeProvider(now);

        var result = await manager.CreateAsync(this.person);

        Assert.True(result.Succeeded);
        Assert.False(person.PasswordLastSet.HasValue);
        Assert.Equal(now, this.person.WhenCreated);
        Assert.Equal(now, this.person.WhenChanged);
        Assert.Equal(now, this.person.PersonWhenChanged);
    }

    [Fact]
    public async Task SetUpdateTimeWhenUpdate()
    {

        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        await manager.CreateAsync(this.person);

        var utcNow = new DateTimeOffset(2023, 11, 4, 3, 50, 34, TimeSpan.Zero);
        manager.TimeProvider = new FrozenTimeProvider(utcNow);

        await manager.UpdateAsync(this.person);

        Assert.Equal(utcNow, person.WhenChanged);
    }

    [Fact]
    public async Task AddPasswordWillSetPasswordLastSetTime()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        await manager.CreateAsync(this.person);

        var result = await manager.AddPasswordAsync(this.person, "Password$1");
        Assert.True(result.Succeeded);
        Assert.NotNull(person.PasswordLastSet);
    }

    [Fact]
    public async Task RemovePassword()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();

        await manager.CreateAsync(this.person, "Pass123$");

        var result = await manager.RemovePasswordAsync(this.person);
        Assert.True(result.Succeeded);
        Assert.Null(this.person.PasswordLastSet);
    }

    [Fact]
    public async Task ChangePassword()
    {
        using var scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        manager.Options.Password.RememberPasswordHistory = 1;

        await manager.CreateAsync(this.person, "Pass123$");

        var result = await manager.ChangePasswordAsync(this.person, "Pass123$", "Pass1234$");
        Assert.True(result.Succeeded);

        var passwordHistoryStore = scope.ServiceProvider.GetRequiredService<IPasswordHistoryStore>();
        var passwords = passwordHistoryStore.GetPasswords(this.person, 10);
        Assert.Single(passwords);

        //change password again with same old password will failed.
        result = await manager.ChangePasswordAsync(this.person, "Pass1234$", "Pass1234$");
        Assert.False(result.Succeeded);
    }
}