using IDSubjects.Subjects;
using System.Diagnostics;
using Xunit;

namespace IDSubjects.Tests;

public class ChineseIdCardNumberTests
{
    [Fact]
    public void CreateChineseIdCardNumberTest()
    {
        var number = new ChineseIdCardNumber(ChineseIdCardNumberVersion.V2, 530302, new DateOnly(1985, 1, 15), 031);
        Assert.Equal(ChineseIdCardNumberVersion.V2, number.Version);
        Assert.Equal(530302, number.RegionCode);
        Assert.Equal(new DateOnly(1985, 1, 15), number.DateOfBirth);
        Assert.Equal(031, number.Sequence);
        Assert.Equal(Gender.Male, number.Gender);
        Assert.Equal("530302198501150314", number.NumberString);
        Assert.Equal("530302850115031", number.ToString(ChineseIdCardNumberVersion.V1));

        //out range test
        Assert.ThrowsAny<Exception>(() => new ChineseIdCardNumber(0, 100000, DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(() => new ChineseIdCardNumber(3, 100000, DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(() => new ChineseIdCardNumber(ChineseIdCardNumberVersion.V1, 99999, DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(() => new ChineseIdCardNumber(ChineseIdCardNumberVersion.V1, 1000000, DateOnly.MinValue, 0));
        Assert.ThrowsAny<Exception>(() => new ChineseIdCardNumber(ChineseIdCardNumberVersion.V1, 100000, DateOnly.MinValue, 1000));
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
        var correctnumber = "530302198501150314";
        Assert.True(ChineseIdCardNumber.TryParse(correctnumber, out _));
        Assert.False(ChineseIdCardNumber.TryParse("530302198501150315", out _));
        Assert.False(ChineseIdCardNumber.TryParse("012345198412100123", out _));
        Assert.False(ChineseIdCardNumber.TryParse("0123", out _));
    }

    [Fact]
    public void ChineseIdCardNumberEqualityTest()
    {
        var number1 = ChineseIdCardNumber.Parse("530302198501150314");
        var number2 = ChineseIdCardNumber.Parse("530302198501150314");
        var number3 = ChineseIdCardNumber.Parse("530302198506020324");
        Assert.True(number1 == number2);
        Assert.False(number1 == number3);
        Assert.True(number1.Equals(number2));
        Assert.True(number2.Equals(number1));
        Assert.False(number1.Equals(number3));
        Assert.Equal(number1.GetHashCode(), number2.GetHashCode());
        Assert.NotEqual(number1.GetHashCode(), number3.GetHashCode());
    }

    [Fact]
    public void GenerateSampleData()
    {
        var liubei = new ChineseIdCardNumber(ChineseIdCardNumberVersion.V2, 990001, new DateOnly(0161, 7, 16), 0001);
        Trace.TraceInformation("刘备的身份证号码是{0}", liubei.ToString());
        var guanyu = new ChineseIdCardNumber(ChineseIdCardNumberVersion.V2, 990001, new DateOnly(0160, 6, 24), 0001);
        Trace.TraceInformation("关羽的身份证号码是{0}", guanyu.ToString());
        var zhangfei = new ChineseIdCardNumber(ChineseIdCardNumberVersion.V2, 990001, new DateOnly(0165, 8, 28), 0001);
        Trace.TraceInformation("张飞的身份证号码是{0}", zhangfei.ToString());
        var zhugeliang = new ChineseIdCardNumber(ChineseIdCardNumberVersion.V2, 990001, new DateOnly(0181, 9, 17), 0001);
        Trace.TraceInformation("诸葛亮的身份证号码是{0}", zhugeliang.ToString());

    }
}
