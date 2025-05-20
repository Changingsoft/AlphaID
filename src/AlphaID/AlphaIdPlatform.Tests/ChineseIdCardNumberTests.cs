using AlphaIdPlatform.Subjects;
using IdSubjects;

namespace AlphaIdPlatform.Tests;

public class ChineseIdCardNumberTests
{
    [Fact]
    public void CreateChineseIdCardNumberTest()
    {
        var number = new ChineseIdCardNumber(ChineseIdCardNumber.V2, "530302", new DateOnly(1985, 1, 15), 031);
        Assert.Equal(ChineseIdCardNumber.V2, number.Version);
        Assert.Equal("530302", number.AreaNumber);
        Assert.Equal(new DateOnly(1985, 1, 15), number.DateOfBirth);
        Assert.Equal(031, number.Sequence);
        Assert.Equal(Gender.Male, number.Gender);
        Assert.Equal("530302198501150314", number.NumberString);
        Assert.Equal("530302850115031", number.ToString(ChineseIdCardNumber.V1));

        //out range test
        Assert.ThrowsAny<Exception>(() => new ChineseIdCardNumber(0, "100000", DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(() => new ChineseIdCardNumber(3, "100000", DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(
            () => new ChineseIdCardNumber(ChineseIdCardNumber.V1, "99999", DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(() =>
            new ChineseIdCardNumber(ChineseIdCardNumber.V1, "1000000", DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(() =>
            new ChineseIdCardNumber(ChineseIdCardNumber.V1, "100000", DateOnly.MinValue, 1000));
    }

    [Fact]
    public void ParseChineseIdCardNumberTest()
    {
        var s = "530302198501150315";
        Assert.ThrowsAny<Exception>(() => ChineseIdCardNumber.Parse(s));
        Assert.ThrowsAny<Exception>(() => ChineseIdCardNumber.Parse("0123"));
    }

    [Fact]
    public void TryParseChineseIdCardNumberTest()
    {
        var correctNumber = "530302198501150314";
        Assert.True(ChineseIdCardNumber.TryParse(correctNumber, out _));
        Assert.False(ChineseIdCardNumber.TryParse("530302198501150315", out _));
        Assert.False(ChineseIdCardNumber.TryParse("012345198412100123", out _));
        Assert.False(ChineseIdCardNumber.TryParse("0123", out _));
    }

    [Fact]
    public void ChineseIdCardNumberEqualityTest()
    {
        ChineseIdCardNumber number1 = ChineseIdCardNumber.Parse("530302198501150314");
        ChineseIdCardNumber number2 = ChineseIdCardNumber.Parse("530302198501150314");
        ChineseIdCardNumber number3 = ChineseIdCardNumber.Parse("530302198506020324");
        Assert.True(number1 == number2);
        Assert.False(number1 == number3);
        Assert.True(number1.Equals(number2));
        Assert.True(number2.Equals(number1));
        Assert.False(number1.Equals(number3));
        Assert.Equal(number1.GetHashCode(), number2.GetHashCode());
        Assert.NotEqual(number1.GetHashCode(), number3.GetHashCode());
    }
}