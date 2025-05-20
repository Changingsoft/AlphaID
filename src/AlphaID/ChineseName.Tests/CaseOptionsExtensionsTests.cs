namespace ChineseName.Tests;

public class CaseOptionsExtensionsTests
{
    [Fact]
    public void NormalizeTest()
    {
        Assert.Equal("A", CaseOptions.FirstLetter.Normalize("a"));
        Assert.Equal("Aa", CaseOptions.FirstLetter.Normalize("aa"));

        Assert.Equal("AA", CaseOptions.Upper.Normalize("aa"));
        Assert.Equal("aa", CaseOptions.Lower.Normalize("AA"));

        Assert.Equal(string.Empty, CaseOptions.Upper.Normalize(string.Empty));
        Assert.Equal("  ", CaseOptions.Lower.Normalize("  "));
    }
}