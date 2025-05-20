using AlphaIdPlatform.Subjects;

namespace AlphaIdPlatform.Tests;

public class UnifiedSocialCreditCodeTests
{
    [Fact]
    public void CreateUscc()
    {
        UnifiedSocialCreditCode code = new("91530300217222042");
        Assert.Equal("91530300217222042J", code.ToString());
        Assert.ThrowsAny<Exception>(() => new UnifiedSocialCreditCode("12345"));
    }

    [Fact]
    public void CreateUsccWithPart()
    {
        var uscc = new UnifiedSocialCreditCode("9", "1", "530300", "217222042");
        Assert.Equal("91530300217222042J", uscc.ToString());
    }

    [Fact]
    public void ParseUscc()
    {
        Assert.ThrowsAny<Exception>(() => UnifiedSocialCreditCode.Parse(""));
        Assert.ThrowsAny<Exception>(() => UnifiedSocialCreditCode.Parse("91530300"));
        Assert.ThrowsAny<Exception>(() => UnifiedSocialCreditCode.Parse("91530300217222042a"));

        UnifiedSocialCreditCode code = UnifiedSocialCreditCode.Parse("91530300217222042j"); //支持对小写的转换。
        Assert.Equal("91530300217222042", code.Code);
        Assert.Equal('J', code.CheckCode);
    }

    [Fact]
    public void TryParseUscc()
    {
        Assert.False(UnifiedSocialCreditCode.TryParse("", out _));
        Assert.False(UnifiedSocialCreditCode.TryParse("123456", out _));
        Assert.False(UnifiedSocialCreditCode.TryParse("9153000061160006XM", out _));
        Assert.True(UnifiedSocialCreditCode.TryParse("91530300217222042j", out UnifiedSocialCreditCode usci));
        Assert.Equal("91530300217222042", usci.Code);
        Assert.Equal('J', usci.CheckCode);
    }

    [Fact]
    public void UsccEquality()
    {
        UnifiedSocialCreditCode a = UnifiedSocialCreditCode.Parse("91530300217222042j");
        UnifiedSocialCreditCode b = UnifiedSocialCreditCode.Parse("91530300217222042J");
        UnifiedSocialCreditCode c = UnifiedSocialCreditCode.Parse("91530300592049026D");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.False(a == c);
    }
}