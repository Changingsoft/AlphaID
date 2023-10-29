using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AlphaIDPlatformTests;
public class CombineTemplatesTest
{
    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "{id?}", "{id?}")]
    [InlineData("", "{id?}", "{id?}")]
    [InlineData("Index", "{id?}", "Index/{id?}")]
    [InlineData("Index", "/{id?}", "{id?}")]
    [InlineData("{anchor}", "/{id?}", "{id?}")]
    [InlineData("{anchor}", "{id?}", "{anchor}/{id?}")]
    public void CombineTemplates(string? prefix, string? template, string? expected)
    {
        var result = AttributeRouteModel.CombineTemplates(prefix, template);
        Assert.Equal(expected, result);
    }
    [Theory]
    [InlineData("", "/Home", "/Home")]
    [InlineData("Area", "", "Area")]
    [InlineData("Area", "/", "Area/")]
    [InlineData("Area", "/Home", "Area/Home")]
    [InlineData("Area", "/Home/", "Area/Home/")]
    public void JoinPath(string a, string b, string expected)
    {
        var result = Path.Join(a, b);
        Assert.Equal(expected, result);
    }
}
