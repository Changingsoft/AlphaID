using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryService;

public class DeleteModel(DirectoryServiceManager directoryServiceManager) : PageModel
{
    public IdSubjects.DirectoryLogon.DirectoryService Data { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Service name")]
    [StringLength(50, ErrorMessage = "Validate_StringLength")]
    public string ServiceName { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int anchor)
    {
        IdSubjects.DirectoryLogon.DirectoryService? svc = await directoryServiceManager.FindByIdAsync(anchor);
        if (svc == null)
            return NotFound();

        Data = svc;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        IdSubjects.DirectoryLogon.DirectoryService? svc = await directoryServiceManager.FindByIdAsync(id);
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

        IdOperationResult result = await directoryServiceManager.DeleteAsync(Data);
        if (!result.Succeeded)
        {
            foreach (string error in result.Errors) ModelState.AddModelError("", error);
            return Page();
        }

        return RedirectToPage("Index");
    }
}