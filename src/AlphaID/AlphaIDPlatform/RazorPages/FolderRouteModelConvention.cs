using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AlphaIDPlatform.RazorPages;

public abstract class FolderRouteModelConvention : IPageRouteModelConvention
{
    public FolderRouteModelConvention(string folderPath, string? areaName = null)
    {
        this.AreaName = areaName;
        this.FolderPath = folderPath.TrimEnd('/');
    }

    public string? AreaName { get; }
    public string FolderPath { get; }

    public void Apply(PageRouteModel model)
    {
        if (string.Equals(this.AreaName, model.AreaName, StringComparison.OrdinalIgnoreCase) &&
            PathBelongsToFolder(this.FolderPath, model.ViewEnginePath))
        {
            this.ApplyRoute(model);
        }
    }
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
