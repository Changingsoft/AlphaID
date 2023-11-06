using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AlphaIDPlatform.RazorPages;

/// <summary>
/// 基于文件夹路径的路由模型约定。
/// </summary>
public abstract class FolderRouteModelConvention : IPageRouteModelConvention
{
    /// <summary>
    /// 使用路径和可选的区域名称初始化约定。
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="areaName"></param>
    protected FolderRouteModelConvention(string folderPath, string? areaName = null)
    {
        this.AreaName = areaName;
        this.FolderPath = folderPath.TrimEnd('/');
    }

    /// <summary>
    /// 获取此约定的区域名称。
    /// </summary>
    public string? AreaName { get; }

    /// <summary>
    /// 获取此约定的路径。
    /// </summary>
    public string FolderPath { get; }

    /// <summary>
    /// 对传入的页面路由模型应用约定。
    /// </summary>
    /// <param name="model"></param>
    public void Apply(PageRouteModel model)
    {
        if (string.Equals(this.AreaName, model.AreaName, StringComparison.OrdinalIgnoreCase) &&
            PathBelongsToFolder(this.FolderPath, model.ViewEnginePath))
        {
            this.ApplyRoute(model);
        }
    }

    /// <summary>
    /// 对传入的页面路由模型应用约定。
    /// </summary>
    /// <param name="model"></param>
    public abstract void ApplyRoute(PageRouteModel model);

    internal static bool PathBelongsToFolder(string folderPath, string viewEnginePath)
    {
        if (folderPath == "/")
        {
            // Root directory covers everything.
            return true;
        }

        return viewEnginePath.Length > folderPath.Length &&
            viewEnginePath.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase) &&
            viewEnginePath[folderPath.Length] == '/';
    }

}
