using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.UserManagement.Pages.Register;

public class RegisterByChineseIdCardModel : PageModel
{
    [BindProperty]
    public string IdCardFrontBase64 { get; set; } = null!;

    [BindProperty]
    public string IdCardBackBase64 { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Phone number")]
    [StringLength(14, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
    public string Mobile { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Validate_EmailFormat")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
    public string? Email { get; set; }

    public void OnGet()
    {
    }

    public Task<IActionResult> OnPostAsync()
    {
        throw new NotImplementedException();
    }
}