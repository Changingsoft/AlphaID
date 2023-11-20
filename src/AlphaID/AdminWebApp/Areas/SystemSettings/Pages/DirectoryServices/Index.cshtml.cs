using IdSubjects.DirectoryLogon;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryServices;

public class IndexModel : PageModel
{
    private readonly DirectoryServiceManager directoryServiceManager;

    public IndexModel(DirectoryServiceManager directoryServiceManager)
    {
        this.directoryServiceManager = directoryServiceManager;
    }

    public IEnumerable<DirectoryService> DirectoryServices { get; set; } = default!;

    public void OnGet()
    {
        this.DirectoryServices = this.directoryServiceManager.Services;
    }
}
