using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryServices;

public class DetailModel : PageModel
{
    private readonly DirectoryServiceManager directoryServiceManager;

    public DetailModel(DirectoryServiceManager directoryServiceManager)
    {
        this.directoryServiceManager = directoryServiceManager;
    }

    public DirectoryService Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var svc = await this.directoryServiceManager.FindByIdAsync(id);
        if (svc == null)
            return this.NotFound();
        this.Data = svc;
        return this.Page();
    }
}
