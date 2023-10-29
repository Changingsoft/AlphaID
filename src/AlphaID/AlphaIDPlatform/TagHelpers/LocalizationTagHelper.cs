using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AlphaIDPlatform.TagHelpers;
internal class LocalizationTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        var content = await output.GetChildContentAsync();

    }
}
