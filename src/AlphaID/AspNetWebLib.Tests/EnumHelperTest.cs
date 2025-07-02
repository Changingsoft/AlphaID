using AspNetWebLib.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AspNetWebLib.Tests;

public class EnumHelperTest
{
    public enum TestEnum
    {
        [Display(Name = "None", Description = "Specify none.")]
        None = 0,

        [Display(Name = "Base line", Description = "Specify base line.")]
        Baseline = 2
    }

    [Fact]
    public void GetListItems()
    {
        IEnumerable<SelectListItem> items = EnumHelper.GetSelectListItems<TestEnum>();
        SelectListItem[] array = items.ToArray();
        Assert.Equal("None", array[0].Text);
        Assert.Equal("None", array[0].Value);
        Assert.Equal("Base line", array[1].Text);
        Assert.Equal("Baseline", array[1].Value);
    }
}