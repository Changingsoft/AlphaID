using IDSubjects.Subjects;
using Xunit;

namespace IDSubjects.Tests;

public class UsciTests
{
    [Fact]
    public void CreateUsciTest()
    {
        UnifiedSocialCreditCode usci = new("91530300217222042");
        Assert.Equal("91530300217222042J", usci.ToString());
        Assert.ThrowsAny<Exception>(() => new UnifiedSocialCreditCode("12345"));
    }

    [Fact]
    public void CreateUsciWithPartTest()
    {
        var uscc = new UnifiedSocialCreditCode("9", "1", "530300", "217222042");
        Assert.Equal("91530300217222042J", uscc.ToString());
    }

    [Fact]
    public void ParseUsciTest()
    {

        Assert.ThrowsAny<Exception>(() => UnifiedSocialCreditCode.Parse(""));
        Assert.ThrowsAny<Exception>(() => UnifiedSocialCreditCode.Parse("91530300"));
        Assert.ThrowsAny<Exception>(() => UnifiedSocialCreditCode.Parse("91530300217222042a"));

        var usci = UnifiedSocialCreditCode.Parse("91530300217222042j"); //支持对小写的转换。
        Assert.Equal("91530300217222042", usci.Code);
        Assert.Equal('J', usci.CheckCode);
    }

    [Fact]
    public void TryParseUsciTest()
    {
        Assert.False(UnifiedSocialCreditCode.TryParse("", out _));
        Assert.False(UnifiedSocialCreditCode.TryParse("123456", out _));
        Assert.False(UnifiedSocialCreditCode.TryParse("9153000061160006XM", out _));
        Assert.True(UnifiedSocialCreditCode.TryParse("91530300217222042j", out UnifiedSocialCreditCode usci));
        Assert.Equal("91530300217222042", usci.Code);
        Assert.Equal('J', usci.CheckCode);
    }

    [Fact]
    public void UsciEqualityTest()
    {
        var a = UnifiedSocialCreditCode.Parse("91530300217222042j");
        var b = UnifiedSocialCreditCode.Parse("91530300217222042J");
        var c = UnifiedSocialCreditCode.Parse("91530300592049026D");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.False(a == c);

    }
}
