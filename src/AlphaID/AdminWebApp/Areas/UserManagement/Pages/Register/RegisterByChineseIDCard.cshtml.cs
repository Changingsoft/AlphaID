using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Register;

public class RegisterByChineseIdCardModel : PageModel
{
    [BindProperty]
    public string IdCardFrontBase64 { get; set; } = default!;

    [BindProperty]
    public string IdCardBackBase64 { get; set; } = default!;

    [Display(Name = "PhoneNumber phone number")]
    [BindProperty]
    public string Mobile { get; set; } = default!;

    [Display(Name = "Email")]
    [EmailAddress]
    [BindProperty]
    public string? Email { get; set; }

    public void OnGet()
    {
    }

    public Task<IActionResult> OnPostAsync()
    {
        throw new NotImplementedException();
    }
}