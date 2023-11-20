using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.SystemSettings.Pages.DirectoryServices;

public class DeleteModel : PageModel
{
    private readonly DirectoryServiceManager directoryServiceManager;

    public DeleteModel(DirectoryServiceManager directoryServiceManager)
    {
        this.directoryServiceManager = directoryServiceManager;
    }

    public DirectoryService Data { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Service name")]
    [StringLength(50, ErrorMessage = "Validate_StringLength")]
    public string ServiceName { get; set; } = default!;

    public async Task<IActionResult> OnGet(int id)
    {
        var svc = await this.directoryServiceManager.FindByIdAsync(id);
        if (svc == null)
            return this.NotFound();

        this.Data = svc;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var svc = await this.directoryServiceManager.FindByIdAsync(id);
        if (svc == null)
            return this.NotFound();
        this.Data = svc;

        if (!this.ModelState.IsValid)
            return this.Page();

        if (this.ServiceName != this.Data.Name)
        {
            this.ModelState.AddModelError(nameof(this.ServiceName), "服务名称不正确");
            return this.Page();
        }

        var result = await this.directoryServiceManager.DeleteAsync(this.Data);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }
        return this.RedirectToPage("Index");
    }
}
