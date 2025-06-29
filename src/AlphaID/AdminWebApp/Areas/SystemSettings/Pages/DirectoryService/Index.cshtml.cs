using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryService;

public class IndexModel(DirectoryServiceManager directoryServiceManager) : PageModel
{
    public IdSubjects.DirectoryLogon.DirectoryService Data { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int anchor)
    {
        IdSubjects.DirectoryLogon.DirectoryService? ds = await directoryServiceManager.FindByIdAsync(anchor);
        if (ds == null)
            return NotFound();
        Data = ds;
        return Page();
    }
}