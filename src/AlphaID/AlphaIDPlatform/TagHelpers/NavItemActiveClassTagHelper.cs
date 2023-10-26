using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AlphaIDPlatform.TagHelpers;

/// <summary>
/// 根据Area和路由前缀，为 Nav Item 的 li 元素添加 active 样式。
/// </summary>
[HtmlTargetElement("li", Attributes = "asp-path")]
[HtmlTargetElement("li", Attributes = "asp-match-prefix")]
[HtmlTargetElement("a", Attributes = "asp-path")]
[HtmlTargetElement("a", Attributes = "asp-match-prefix")]
public class NavItemActiveClassTagHelper : TagHelper
{

    /// <summary>
    /// 
    /// </summary>
    [HtmlAttributeName("asp-path")]
    public string Path { get; set; } = "/";

    /// <summary>
    /// 
    /// </summary>
    [HtmlAttributeName("asp-match-prefix")]
    public bool MatchPrefix { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var currentPath = this.ViewContext.HttpContext.Request.Path.Value;
        bool result;
        if (this.MatchPrefix)
            result = currentPath!.StartsWith(this.Path, StringComparison.OrdinalIgnoreCase);
        else
            result = string.Equals(this.Path, currentPath, StringComparison.OrdinalIgnoreCase);

        if (result)
        {
            var existingClasses = output.Attributes["class"].Value.ToString();
            if (output.Attributes["class"] != null)
            {
                output.Attributes.Remove(output.Attributes["class"]);
            }

            output.Attributes.Add("class", $"{existingClasses} active");
        }
    }
}
