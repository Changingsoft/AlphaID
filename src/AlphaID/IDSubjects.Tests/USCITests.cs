using IDSubjects.Subjects;
using Xunit;

namespace IDSubjectsTests;

public class USCITests
{
    [Fact]
    public void CreateUSCITest()
    {
        USCC usci = new("91530300217222042");
        Assert.Equal("91530300217222042J", usci.ToString());
        Assert.ThrowsAny<Exception>(() => new USCC("12345"));
    }

    [Fact]
    public void CreateUSCIWithPartTest()
    {
        var uscc = new USCC("9", "1", "530300", "217222042");
        Assert.Equal("91530300217222042J", uscc.ToString());
    }

    [Fact]
    public void ParseUSCITest()
    {

        Assert.ThrowsAny<Exception>(() => USCC.Parse(""));
        Assert.ThrowsAny<Exception>(() => USCC.Parse("91530300"));
        Assert.ThrowsAny<Exception>(() => USCC.Parse("91530300217222042a"));

        var usci = USCC.Parse("91530300217222042j"); //支持对小写的转换。
        Assert.Equal("91530300217222042", usci.Code);
        Assert.Equal('J', usci.CheckCode);
    }

    [Fact]
    public void TryParseUSCITest()
    {
        Assert.False(USCC.TryParse("", out _));
        Assert.False(USCC.TryParse("123456", out _));
        Assert.False(USCC.TryParse("9153000061160006XM", out _));
        Assert.True(USCC.TryParse("91530300217222042j", out USCC usci));
        Assert.Equal("91530300217222042", usci.Code);
        Assert.Equal('J', usci.CheckCode);
    }

    [Fact]
    public void USCIEqualityTest()
    {
        var a = USCC.Parse("91530300217222042j");
        var b = USCC.Parse("91530300217222042J");
        var c = USCC.Parse("91530300592049026D");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.False(a == c);

    }
}
