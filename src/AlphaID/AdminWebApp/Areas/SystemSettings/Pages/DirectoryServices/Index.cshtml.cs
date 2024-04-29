using IdSubjects.DirectoryLogon;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryServices;

public class IndexModel(DirectoryServiceManager directoryServiceManager) : PageModel
{
    public IEnumerable<DirectoryServiceDescriptor> DirectoryServices { get; set; } = default!;

    public void OnGet()
    {
        DirectoryServices = directoryServiceManager.Services;
    }
}