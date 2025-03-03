using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetWebLib.RazorPages;

/// <summary>
/// 主语标识锚点路由模型约定
/// </summary>
/// <remarks>
/// <para>
/// 该约定将在指定的路径为界，将主语参数加入路径中，然后再拼接尾随路径和其他路由模板，从而组成更具有语义的友好URL。
/// </para>
/// </remarks>
/// <remarks>
/// 使用路径、可选的区域名称和锚点模板初始化约定。
/// </remarks>
/// <param name="folderPath">路径名称。路径名称应总是以“/”开头，不要添加结尾的“/”。</param>
/// <param name="areaName">可选的区域名称。</param>
/// <param name="anchorTemplate">锚点模板。</param>
public class SubjectAnchorRouteModelConvention(
    string folderPath,
    string? areaName = null,
    string anchorTemplate = "{anchor}") : FolderRouteModelConvention(folderPath, areaName)
{
    /// <summary>
    /// 获取锚点模板。
    /// </summary>
    public string AnchorTemplate { get; } = anchorTemplate;

    /// <summary>
    /// 应用此约定。
    /// </summary>
    /// <param name="model"></param>
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

        // passed if route template is override pattern

        string pathPrefix = Path.Join(AreaName ?? "", FolderPath).Trim('/');

        foreach (SelectorModel selector in model.Selectors)
        {
            PageRouteMetadata? metadata = selector.EndpointMetadata.OfType<PageRouteMetadata>().SingleOrDefault();
            if (metadata == null ||
                AttributeRouteModel.IsOverridePattern(metadata.RouteTemplate)) //没有元数据，或路由模板是覆盖模式时，跳过处理
                continue;
            Debug.Assert(!selector.AttributeRouteModel!.Template!.StartsWith('~'));
            string absTemplate = selector.AttributeRouteModel!.Template!;
            Debug.Assert(absTemplate.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase));
            string left = absTemplate[..pathPrefix.Length];
            string right = absTemplate[pathPrefix.Length..].Trim('/');
            string? newPathPrefix = AttributeRouteModel.CombineTemplates(left, AnchorTemplate);
            selector.AttributeRouteModel.Template = AttributeRouteModel.CombineTemplates(newPathPrefix, right);
            selector.EndpointMetadata.Clear();
        }
    }
}