using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetWebLib.TagHelpers;

/// <summary>
/// </summary>
[HtmlTargetElement("div", Attributes = ForAttributeName)]
[HtmlTargetElement("p", Attributes = ForAttributeName)]
[HtmlTargetElement("span", Attributes = ForAttributeName)]
public sealed class DescriptionForTagHelper : TagHelper
{
    private const string ForAttributeName = "asp-description-for";

    /// <summary>
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression For { get; set; } = null!;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        string? description = For.Metadata.Description;
        if (description != null)
            // Do not update the content if another tag helper
            // targeting this element has already done so.
            if (!output.IsContentModified)
            {
                TagHelperContent? childContent = await output.GetChildContentAsync();
                if (childContent.IsEmptyOrWhiteSpace)
                    output.Content.SetHtmlContent(description);
                else
                    output.Content.SetHtmlContent(childContent);
            }
    }
}