using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Diagnostics;

namespace AlphaIDPlatform.RazorPages;

public class SubjectAnchorRouteModelConvention : FolderRouteModelConvention
{
    public SubjectAnchorRouteModelConvention(string folderPath, string? areaName = null, string anchorTemplate = "{anchor}")
        : base(folderPath, areaName) => this.AnchorTemplate = anchorTemplate;

    public string AnchorTemplate { get; }

    public override void ApplyRoute(PageRouteModel model)
    {
        // Example Reference
        // 输入：
        // RelativePath = "/Areas/Products/Pages/Manage/Index.cshtml"
        // RouteTemplate = "{param}"
        // 处理：
        // ViewEnginePath = "/Manage/Index"
        // AreaName = "Products"
        // 
        // PageRoute = "/" + AreaName + ViewEnginePath = "/Products/Manage/Index"
        // RouteValues["page"] --> "/Manage/Index"
        //
        // Create a SelectorModel: AttributeRouteModel.Template --> PageRoute & RouteTemplate           --> "Products/Manage/Index/{param}"
        //                         EndpointMetadata[0]-->PageRouteMetadata(PageRoute, RouteTemplate)
        //
        // If RelativePath ending with "Index.cshtml", then add SelectorModel for ParentDirectory like "/Products/Manage"
        // Create a SelectorModel: AttributeRouteModel.Template --> ParentDirectory & RouteTemplate     --> "Products/Manage/{param}"
        //                         EndpointMetadata[0]-->PageRouteMetadata(PageRoute, RouteTemplate)
        // then set suppress generate link to first selector.

        // 处理目的：将anchor插入路径中，例如：
        // 传入 RelativePath = "/Areas/Products/Pages/Manage/Index.cshtml"
        // 传入 RouteTemplate = "{param}"
        // 设置 AreaName = "Products"
        // 设置 FolderPath = "/"
        //
        // Result Template is: "Products/{anchor}/Manage/Index/{param}"
        //                     "Products/{anchor}/Manage/{param}"
        // !at last, clear metadata force use our template.
        //
        // 0         1         2         3         4
        // 01234567890123456789012345678901234567890
        // Products/Manage/Index/{param}
        // Products/{anchor}/Manage/Index/{param}
        //         /\       /\

        // passed if route templat is override pattern

        var pathPrefix = Path.Join(this.AreaName ?? "", this.FolderPath).Trim('/');

        for (var i = 0; i < model.Selectors.Count; i++)
        {
            var selector = model.Selectors[i];
            var metadata = selector.EndpointMetadata.OfType<PageRouteMetadata>().SingleOrDefault();
            if (metadata == null || AttributeRouteModel.IsOverridePattern(metadata.RouteTemplate)) //没有元数据，或路由模板是覆盖模式时，跳过处理
                continue;
            Debug.Assert(!selector.AttributeRouteModel!.Template!.StartsWith("~"));
            var absTemplate = selector!.AttributeRouteModel!.Template!;
            Debug.Assert(absTemplate.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase));
            var left = absTemplate[..pathPrefix.Length];
            var right = absTemplate[pathPrefix.Length..].Trim('/');
            var newPathPrefix = AttributeRouteModel.CombineTemplates(left, this.AnchorTemplate);
            selector.AttributeRouteModel.Template = AttributeRouteModel.CombineTemplates(newPathPrefix, right);
            selector.EndpointMetadata.Clear();
        }

    }
}
