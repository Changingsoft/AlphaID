using IdSubjects.Validators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdSubjects.Tests;
[Collection(nameof(ServiceProviderCollection))]
public class AlphaIdPasswordValidatorTest(ServiceProviderFixture serviceProvider)
{
    [Fact]
    public async Task TestPasswordComplexityRule()
    {
        using IServiceScope scope = serviceProvider.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ApplicationUserManager<ApplicationUser>>();
        manager.Options.Tokens.PasswordResetTokenProvider = "Static";
        manager.RegisterTokenProvider("Static", new StaticTokenProvider());

        var user = new ApplicationUser("TestUser");
        //Arrange
        var validator = manager.PasswordValidators.OfType<AlphaIdPasswordValidator<ApplicationUser>>().First();
        //Assert
        Assert.False((await validator.ValidateAsync(manager, user, "ABCDEFGH")).Succeeded); //大写
        Assert.False((await validator.ValidateAsync(manager, user, "abcdefgh")).Succeeded); //小写
        Assert.False((await validator.ValidateAsync(manager, user, "12345678")).Succeeded); //数字
        Assert.False((await validator.ValidateAsync(manager, user, "!@#$%^&*")).Succeeded); //特殊

        Assert.False((await validator.ValidateAsync(manager, user, "Abcdefgh")).Succeeded); //大写、小写
        Assert.False((await validator.ValidateAsync(manager, user, "ABCDEFG1")).Succeeded); //大写、数字
        Assert.False((await validator.ValidateAsync(manager, user, "ABCDEFG!")).Succeeded); //大写、特殊
        Assert.False((await validator.ValidateAsync(manager, user, "abcdefg1")).Succeeded); //小写、数字
        Assert.False((await validator.ValidateAsync(manager, user, "abcdefg!")).Succeeded); //小写、特殊
        Assert.False((await validator.ValidateAsync(manager, user, "1234567!")).Succeeded); //数字、特殊

        Assert.True((await validator.ValidateAsync(manager, user, "Abcdefg1")).Succeeded); //大写、小写、数字
        Assert.True((await validator.ValidateAsync(manager, user, "Abcdefg!")).Succeeded); //大写、小写、特殊
        Assert.True((await validator.ValidateAsync(manager, user, "ABCDEF1!")).Succeeded); //大写、数字、特殊
        Assert.True((await validator.ValidateAsync(manager, user, "abcdef1!")).Succeeded); //小写、数字、特殊
    }
}
