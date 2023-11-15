using AlphaIdPlatform.RazorPages;

namespace AlphaIdPlatform.Tests;
public class FolderRouteModelConventionTest
{
    [Theory]
    [InlineData("", "/Index", true)]
    [InlineData("/", "/Index", true)]
    [InlineData("/I", "/Index", false)]
    [InlineData("Index", "/Index", false)]
    [InlineData("Home", "/Home/Index", false)]
    [InlineData("/Hom", "/Home/Index", false)]
    [InlineData("/Home", "/Home/Index", true)]
    [InlineData("/Home/", "/Home/Index", true)]
    [InlineData("/Home/I", "/Home/Index", false)]
    public void PathBelongs(string folder, string viewEnginePath, bool expect)
    {
        folder = folder.TrimEnd('/');
        var result = FolderRouteModelConvention.PathBelongsToFolder(folder, viewEnginePath);
        Assert.Equal(expect, result);
    }
}
