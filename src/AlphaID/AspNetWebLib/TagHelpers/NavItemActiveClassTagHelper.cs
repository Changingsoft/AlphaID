using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetWebLib.TagHelpers;

/// <summary>
/// 根据Area和路由前缀，为 Nav Item 的 li 元素添加 active 样式。
/// </summary>
[HtmlTargetElement("li", Attributes = "asp-path")]
[HtmlTargetElement("a", Attributes = "asp-path")]
public class NavItemActiveClassTagHelper : TagHelper
{
    /// <summary>
    /// 将激活附加 active 样式的路径。路径必须以“/”开头，不能以“/”结尾。如果要严格匹配，在末尾添加“!”。
    /// </summary>
    [HtmlAttributeName("asp-path")]
    public string Path { get; set; } = "/";

    /// <summary>
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        string? currentPath = ViewContext.HttpContext.Request.Path.Value;
        if (currentPath == null)
            return;
        bool restrictMatch = false;
        string targetPath = Path;
        if (Path.EndsWith('!'))
        {
            restrictMatch = true;
            targetPath = targetPath[..^1];
        }
        bool matched = restrictMatch
            ? string.Equals(targetPath, currentPath, StringComparison.OrdinalIgnoreCase)
            : currentPath.StartsWith(targetPath, StringComparison.OrdinalIgnoreCase);

        if (!matched) return;

        string? existingClasses = output.Attributes["class"].Value.ToString();
        if (output.Attributes["class"] != null) output.Attributes.Remove(output.Attributes["class"]);

        output.Attributes.Add("class", $"{existingClasses} active");
    }
}