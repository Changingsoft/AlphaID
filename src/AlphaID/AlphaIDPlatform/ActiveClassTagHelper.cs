using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AlphaIDPlatform;

[HtmlTargetElement(Attributes = "is-active-route")]
public class ActiveClassTagHelper : AnchorTagHelper
{
    public ActiveClassTagHelper(IHtmlGenerator generator) : base(generator)
    {
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var routeData = this.ViewContext.RouteData.Values;
        var currentController = routeData["controller"] as string;
        var currentAction = routeData["action"] as string;
        var currentArea = routeData["area"] as string;
        var currentPage = routeData["page"] as string;
        var result = false;

        if (!string.IsNullOrWhiteSpace(this.Controller) && !String.IsNullOrWhiteSpace(this.Action))
        {
            result = string.Equals(this.Action, currentAction, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this.Controller, currentController, StringComparison.OrdinalIgnoreCase);
        }
        else if (!string.IsNullOrWhiteSpace(this.Action))
        {
            result = string.Equals(this.Action, currentAction, StringComparison.OrdinalIgnoreCase);
        }
        else if (!string.IsNullOrWhiteSpace(this.Controller))
        {
            result = string.Equals(this.Controller, currentController, StringComparison.OrdinalIgnoreCase);
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
