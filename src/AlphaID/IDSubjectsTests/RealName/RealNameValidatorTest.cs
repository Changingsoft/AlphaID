using IDSubjects;
using IDSubjects.RealName;
using IDSubjects.Subjects;
using IDSubjectsTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IDSubjectsTests.RealName;

[Collection("ServiceProviderCollection")]
public class RealNameValidatorTest : IClassFixture<NaturalPersonMocker>
{
    private readonly ServiceProviderFixture fixture;
    private readonly NaturalPersonMocker naturalPersonMocker;

    public RealNameValidatorTest(ServiceProviderFixture fixture, NaturalPersonMocker naturalPersonMocker)
    {
        this.fixture = fixture;
        this.naturalPersonMocker = naturalPersonMocker;
    }

    [Fact]
    public async Task CommitValidationRequest()
    {
        //Prepare
        using var scope = this.fixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);
        var validator = scope.ServiceProvider.GetRequiredService<ChineseIDCardManager>();

        //Execute
        var chineseIDCardImage = new ChineseIDCardImage(Array.Empty<byte>(), "image/jpeg", Array.Empty<byte>(), "image/jpeg");
        var validation = new ChineseIDCardValidation(chineseIDCardImage);
        await validator.CommitAsync(person, validation);

        //Test
        var pending = await validator.GetPendingRequestAsync(person);
        Assert.Equal(validation, pending);
    }

    [Fact]
    public async Task ApplyChineseIDCardInfo()
    {
        //Prepare
        using var scope = this.fixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);
        var validator = scope.ServiceProvider.GetRequiredService<ChineseIDCardManager>();

        //Execute
        var chineseIDCardImage = new ChineseIDCardImage(Array.Empty<byte>(), "image/jpeg", Array.Empty<byte>(), "image/jpeg");
        var validation = new ChineseIDCardValidation(chineseIDCardImage);
        await validator.CommitAsync(person, validation);

        var tryApplyResult = validation.TryApplyChineseIDCardInfo(new ChineseIDCardInfo("张三",
                                                                   Sex.Male,
                                                                   "汉",
                                                                   new DateTime(2000, 1, 1),
                                                                   "Address",
                                                                   "100000200001011239",
                                                                   "Beijing",
                                                                   new DateTime(2015, 1, 1)));
        Assert.True(tryApplyResult);

        await validator.UpdateAsync(validation);

        //Test
        var pending = await validator.GetPendingRequestAsync(person);
        Assert.Equal(validation, pending);
        Assert.NotNull(validation.ChineseIDCard);
    }

    [Fact]
    public async Task TryApplyChinesePersonNameAsync()
    {
        //Prepare
        using var scope = this.fixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);
        var validator = scope.ServiceProvider.GetRequiredService<ChineseIDCardManager>();

        //Execute
        var chineseIDCardImage = new ChineseIDCardImage(Array.Empty<byte>(), "image/jpeg", Array.Empty<byte>(), "image/jpeg");
        var validation = new ChineseIDCardValidation(chineseIDCardImage);
        await validator.CommitAsync(person, validation);

        var tryApplyResult = validation.TryApplyChinesePersonName(new ChinesePersonName("张", "三", "ZHANG", "SAN"));
        Assert.True(tryApplyResult);

        await validator.UpdateAsync(validation);

        //Test
        var pending = await validator.GetPendingRequestAsync(person);
        Assert.Equal(validation, pending);
        Assert.NotNull(validation.ChinesePersonName);
    }

    [Fact]
    public async Task AcceptValidationAsync()
    {
        //Prepare
        using var scope = this.fixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);
        var validator = scope.ServiceProvider.GetRequiredService<ChineseIDCardManager>();

        var chineseIDCardImage = new ChineseIDCardImage(Array.Empty<byte>(), "image/jpeg", Array.Empty<byte>(), "image/jpeg");
        var validation = new ChineseIDCardValidation(chineseIDCardImage);
        await validator.CommitAsync(person, validation);
        validation.TryApplyChineseIDCardInfo(new ChineseIDCardInfo("张三",
                                                                   Sex.Male,
                                                                   "汉",
                                                                   new DateTime(2000, 1, 1),
                                                                   "Address",
                                                                   "100000200001011239",
                                                                   "Beijing",
                                                                   new DateTime(2015, 1, 1),
                                                                   new DateTime(2025, 1, 1)));
        validation.TryApplyChinesePersonName(new ChinesePersonName("张", "三", "ZHANG", "SAN"));
        await validator.UpdateAsync(validation);
        //Execute
        await validator.ValidateAsync(validation, "Validator", true);


        //Test
        Assert.NotNull(validation.Result);
        var pending = await validator.GetPendingRequestAsync(person);
        Assert.Null(pending);
        var current = await validator.GetCurrentAsync(person);
        Assert.Equal(validation, current);
    }
}
