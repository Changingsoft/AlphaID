using IDSubjects.Subjects;
using Xunit;

namespace IDSubjects.Tests;

public class UsciTests
{
    [Fact]
    public void CreateUsciTest()
    {
        Uscc usci = new("91530300217222042");
        Assert.Equal("91530300217222042J", usci.ToString());
        Assert.ThrowsAny<Exception>(() => new Uscc("12345"));
    }

    [Fact]
    public void CreateUsciWithPartTest()
    {
        var uscc = new Uscc("9", "1", "530300", "217222042");
        Assert.Equal("91530300217222042J", uscc.ToString());
    }

    [Fact]
    public void ParseUsciTest()
    {

        Assert.ThrowsAny<Exception>(() => Uscc.Parse(""));
        Assert.ThrowsAny<Exception>(() => Uscc.Parse("91530300"));
        Assert.ThrowsAny<Exception>(() => Uscc.Parse("91530300217222042a"));

        var usci = Uscc.Parse("91530300217222042j"); //支持对小写的转换。
        Assert.Equal("91530300217222042", usci.Code);
        Assert.Equal('J', usci.CheckCode);
    }

    [Fact]
    public void TryParseUsciTest()
    {
        Assert.False(Uscc.TryParse("", out _));
        Assert.False(Uscc.TryParse("123456", out _));
        Assert.False(Uscc.TryParse("9153000061160006XM", out _));
        Assert.True(Uscc.TryParse("91530300217222042j", out Uscc usci));
        Assert.Equal("91530300217222042", usci.Code);
        Assert.Equal('J', usci.CheckCode);
    }

    [Fact]
    public void UsciEqualityTest()
    {
        var a = Uscc.Parse("91530300217222042j");
        var b = Uscc.Parse("91530300217222042J");
        var c = Uscc.Parse("91530300592049026D");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.False(a == c);

    }
}
