using AlphaIDPlatform.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AlphaIDPlatformTests;
public class EnumHelperTest
{
    [Fact]
    public void GetListItems()
    {
        var items = EnumHelper.GetSelectListItems<TestEnum>();
        var array = items.ToArray();
        Assert.Equal("None", array[0].Text);
        Assert.Equal("None", array[0].Value);
        Assert.Equal("Base line", array[1].Text);
        Assert.Equal("Baseline", array[1].Value);
    }

    public enum TestEnum
    {
        [Display(Name = "None", Description = "Specify none.")]
        None = 0,
        [Display(Name = "Base line", Description = "Specify base line.")]
        Baseline = 2,
    }
}
