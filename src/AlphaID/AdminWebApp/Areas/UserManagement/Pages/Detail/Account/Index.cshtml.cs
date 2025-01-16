using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class IndexModel(ApplicationUserManager userManager) : PageModel
{
    public ApplicationUser Data { get; set; } = null!;

    public bool HasPassword { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public string? OperationResultMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        ApplicationUser? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Data = person;
        HasPassword = await userManager.HasPasswordAsync(Data);
        Input = new InputModel
        {
            UserName = Data.UserName
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        ApplicationUser? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Data = person;
        HasPassword = await userManager.HasPasswordAsync(Data);

        if (!ModelState.IsValid) return Page();

        if (userManager.Users.Any(p => p.Id != Data.Id && p.UserName == Input.UserName))
        {
            ModelState.AddModelError("", "�����������˻�����ͬ");
            return Page();
        }

        await userManager.SetUserNameAsync(Data, Input.UserName);
        OperationResultMessage = "�����ѳɹ���";
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "User name")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string UserName { get; set; } = null!;
    }
}