using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AlphaIDPlatform;

/// <summary>
/// 根据Area和路由前缀，为 Nav Item 的 li 元素添加 active 样式。
/// </summary>
[HtmlTargetElement("li", Attributes = "asp-area")]
[HtmlTargetElement("li", Attributes = "asp-path-prefix")]
public class NavItemActiveClassTagHelper : TagHelper
{
    /// <summary>
    /// 
    /// </summary>
    [HtmlAttributeName("asp-area")]
    public string? Area { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [HtmlAttributeName("asp-path-prefix")]
    public string PathPrefix { get; set; } = "/Index";

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
        var routeData = this.ViewContext.RouteData.Values;
        var currentController = routeData["controller"] as string;
        var currentAction = routeData["action"] as string;
        var currentArea = routeData["area"] as string ?? "";
        var currentPage = routeData["page"] as string ?? "";
        var currentPath = this.ViewContext.HttpContext.Request.Path;
        var result = false;

        if (this.Area != null)
        {
            result = string.Equals(this.Area, currentArea, StringComparison.OrdinalIgnoreCase) && currentPage.StartsWith(this.PathPrefix, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            result = currentPage.StartsWith(this.PathPrefix, StringComparison.OrdinalIgnoreCase);
        }

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
