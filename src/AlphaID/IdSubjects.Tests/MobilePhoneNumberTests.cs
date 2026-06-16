using Xunit;

namespace IdSubjects.Tests;

public class MobilePhoneNumberTests
{
    [Theory]
    [InlineData("86", "15912345678")]
    public void InitializeTest(string countryCode, string phoneNumber)
    {
        var number1 = new MobilePhoneNumber(countryCode, phoneNumber);
        Assert.Equal(countryCode, number1.CountryCode);
        Assert.Equal(phoneNumber, number1.PhoneNumber);
    }

    [Theory]
    [InlineData(" ", " ")]
    [InlineData("86", "1381234567a")]
    public void InitThrowsException(string countryCode, string phoneNumber)
    {
        Assert.ThrowsAny<Exception>(() => new MobilePhoneNumber(countryCode, phoneNumber));
    }

    [Fact]
    public void ToStringTest()
    {
        var number = new MobilePhoneNumber("86", "15912345678");
        Assert.Equal("+8615912345678", number.ToString());
    }

    [Fact]
    public void TryParseMobilePhoneNumber()
    {
        Assert.True(MobilePhoneNumber.TryParse("+8615912345678", out MobilePhoneNumber number1));
        Assert.Equal("15912345678", number1.PhoneNumber);

        Assert.True(MobilePhoneNumber.TryParse("13812345678", out MobilePhoneNumber number2));
        Assert.Equal("13812345678", number2.PhoneNumber);

        Assert.True(MobilePhoneNumber.TryParse("+8613812345678 ", out _));

        Assert.False(MobilePhoneNumber.TryParse("+8513812345678", out _));
        //todo 不再作长度检查。
        //Assert.False(MobilePhoneNumber.TryParse("+86138123456789", out _));
        //Assert.False(MobilePhoneNumber.TryParse("+861381234567", out _));
        Assert.False(MobilePhoneNumber.TryParse("", out _));
        Assert.False(MobilePhoneNumber.TryParse("  ", out _));
    }

    [Fact]
    public void ParseMobilePhoneNumber()
    {
        MobilePhoneNumber number1 = MobilePhoneNumber.Parse("+8615912345678");
        Assert.Equal("15912345678", number1.PhoneNumber);

        MobilePhoneNumber number2 = MobilePhoneNumber.Parse(" +8613812345678 ");
        Assert.Equal("13812345678", number2.PhoneNumber);

        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse(""));
        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("  "));
        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("+861a59123456789"));
        //todo 当前已不作手机号长度检查，因此以下两条测试不再抛出异常，后续如果需要恢复手机号长度检查，则需要恢复这两条测试。
        //Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("+861a591234567"));
        //Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("+851a5912345678"));
    }
}