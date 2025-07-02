using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.UserManagement.Pages.Register;

public class RegisterByChineseIdCardModel : PageModel
{
    [BindProperty]
    public string IdCardFrontBase64 { get; set; } = null!;

    [BindProperty]
    public string IdCardBackBase64 { get; set; } = null!;

    [Display(Name = "PhoneNumber phone number")]
    [BindProperty]
    public string Mobile { get; set; } = null!;

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