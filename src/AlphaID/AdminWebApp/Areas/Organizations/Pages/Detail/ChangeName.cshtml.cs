using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class ChangeNameModel : PageModel
{
    private readonly OrganizationManager manager;

    public ChangeNameModel(OrganizationManager manager)
    {
        this.manager = manager;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var org = await this.manager.FindByIdAsync(this.Id);
        if (org == null)
            return this.NotFound();

        this.Input = new()
        {
            CurrentName = org.Name,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var org = await this.manager.FindByIdAsync(this.Id);
        if (org == null)
            return this.NotFound();

        var result = await this.manager.ChangeNameAsync(org, this.Input.NewName, this.Input.ChangeDate, this.Input.RecordUsedName, this.Input.ApplyChangeWhenNameDuplicated);
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }

        return this.RedirectToPage("Detail", new { id = this.Id });
    }

    public class InputModel
    {
        [Display(Name = "当前名称")]
        public string CurrentName { get; set; } = default!;

        [Display(Name = "新名称")]
        [StringLength(100, MinimumLength = 4)]
        public string NewName { get; set; } = default!;

        [DataType(DataType.Date)]
        [Display(Name = "更名日期")]
        public DateTime ChangeDate { get; set; } = DateTime.Now.Date;

        [Display(Name = "当前名称计入曾用名")]
        public bool RecordUsedName { get; set; } = true;

        [Display(Name = "即使名称重复也要变更")]
        public bool ApplyChangeWhenNameDuplicated { get; set; } = false;
    }
}
