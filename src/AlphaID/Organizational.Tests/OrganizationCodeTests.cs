namespace Organizational.Tests;

public class OrganizationCodeTests
{
    [Fact]
    public void CreateOrganizationCodeTest()
    {
        OrganizationCode usci = new("21722204");
        Assert.Equal("21722204-2", usci.ToString());
        Assert.ThrowsAny<Exception>(() => new OrganizationCode("12345"));
    }

    [Fact]
    public void ParseOrganizationCodeTest()
    {
        Assert.ThrowsAny<Exception>(() => OrganizationCode.Parse(""));
        Assert.ThrowsAny<Exception>(() => OrganizationCode.Parse("12345"));
        Assert.ThrowsAny<Exception>(() => OrganizationCode.Parse("21722204-3"));

        var oc = OrganizationCode.Parse("21722204-2"); //支持对小写的转换。
        Assert.Equal("21722204", oc.Code);
        Assert.Equal('2', oc.CheckCode);

        var oc1 = OrganizationCode.Parse("217222042"); //支持对小写的转换。
        Assert.Equal("21722204", oc1.Code);
        Assert.Equal('2', oc1.CheckCode);
    }

    [Fact]
    public void TryParseOrganizationCodeTest()
    {
        Assert.False(OrganizationCode.TryParse("", out _));
        Assert.False(OrganizationCode.TryParse("123456", out _));
        Assert.True(OrganizationCode.TryParse("21722204-2", out _));
        Assert.True(OrganizationCode.TryParse("217222042", out OrganizationCode oc));
        Assert.Equal("21722204", oc.Code);
        Assert.Equal('2', oc.CheckCode);
    }

    [Fact]
    public void OrganizationCodeEqualityTest()
    {
        var a = OrganizationCode.Parse("21722204-2");
        var b = OrganizationCode.Parse("217222042");
        var c = OrganizationCode.Parse("70978816-5");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.False(a == c);
    }
}