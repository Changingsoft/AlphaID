using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryService;

public class DeleteModel(DirectoryServiceManager directoryServiceManager) : PageModel
{
    public DirectoryServiceDescriptor Data { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Service name")]
    [StringLength(50, ErrorMessage = "Validate_StringLength")]
    public string ServiceName { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int anchor)
    {
        var svc = await directoryServiceManager.FindByIdAsync(anchor);
        if (svc == null)
            return NotFound();

        Data = svc;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var svc = await directoryServiceManager.FindByIdAsync(id);
        if (svc == null)
            return NotFound();
        Data = svc;

        if (!ModelState.IsValid)
            return Page();

        if (ServiceName != Data.Name)
        {
            ModelState.AddModelError(nameof(ServiceName), "服务名称不正确");
            return Page();
        }

        var result = await directoryServiceManager.DeleteAsync(Data);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return Page();
        }
        return RedirectToPage("Index");
    }
}
