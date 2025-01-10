using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryService;

public class IndexModel(DirectoryServiceManager directoryServiceManager) : PageModel
{
    public DirectoryServiceDescriptor Data { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int anchor)
    {
        DirectoryServiceDescriptor? ds = await directoryServiceManager.FindByIdAsync(anchor);
        if (ds == null)
            return NotFound();
        Data = ds;
        return Page();
    }
}