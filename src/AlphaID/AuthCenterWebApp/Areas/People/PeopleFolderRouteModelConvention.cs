using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AuthCenterWebApp.Areas.People;

public class PeopleFolderRouteModelConvention : IPageRouteModelConvention
{
    const string areaName = "People";

    const string folderPath = "/";

    public void Apply(PageRouteModel model)
    {
        if(string.Equals(areaName, model.AreaName, StringComparison.OrdinalIgnoreCase) &&
            PathBelongsToFolder(folderPath, model.ViewEnginePath))
        {
            this.ApplyRoute(model);
        }
    }

    void ApplyRoute(PageRouteModel model)
    {
        //todo 重写路由
    }

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
