using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class IndexModel(NaturalPersonManager userManager) : PageModel
{
    public NaturalPerson Data { get; set; } = default!;

    public bool HasPassword { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public string? OperationResultMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
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
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Data = person;
        HasPassword = await userManager.HasPasswordAsync(Data);

        if (!ModelState.IsValid) return Page();

        if (userManager.Users.Any(p => p.Id != Data.Id && p.UserName == Input.UserName))
        {
            ModelState.AddModelError("", "不能与其他账户名相同");
            return Page();
        }

        await userManager.SetUserNameAsync(Data, Input.UserName);
        OperationResultMessage = "操作已成功。";
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "User name")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string UserName { get; set; } = default!;
    }
}