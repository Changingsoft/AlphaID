using IDSubjects.Subjects;
using Xunit;

namespace IDSubjects.Tests;

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
        Assert.False(MobilePhoneNumber.TryParse("+86138123456789", out _));
        Assert.False(MobilePhoneNumber.TryParse("+861381234567", out _));
        Assert.False(MobilePhoneNumber.TryParse("", out _));
        Assert.False(MobilePhoneNumber.TryParse("  ", out _));
    }

    [Fact]
    public void ParseMobilePhoneNumber()
    {
        var number1 = MobilePhoneNumber.Parse("+8615912345678");
        Assert.Equal("15912345678", number1.PhoneNumber);

        var number2 = MobilePhoneNumber.Parse(" +8613812345678 ");
        Assert.Equal("13812345678", number2.PhoneNumber);

        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse(""));
        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("  "));
        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("+86159123456789"));
        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("+861591234567"));
        Assert.ThrowsAny<Exception>(() => MobilePhoneNumber.Parse("+8515912345678"));
    }
}
