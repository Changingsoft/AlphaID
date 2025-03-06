using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class ContactsModel(UserManager<NaturalPerson> manager, NaturalPersonService naturalPersonService) : PageModel
{
    public NaturalPerson Data { get; set; } = null!;

    [Display(Name = "Phone number")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Validate_StringLength")]
    [DataType(DataType.PhoneNumber)]
    [BindProperty]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Email")]
    [StringLength(100, ErrorMessage = "Validate_StringLength")]
    [EmailAddress]
    [BindProperty]
    public string? Email { get; set; }


    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? data = await manager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();

        Data = data;
        Email = Data.Email;
        if (Data.PhoneNumber != null && MobilePhoneNumber.TryParse(Data.PhoneNumber, out var number))
        {
            PhoneNumber = number.PhoneNumber;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        NaturalPerson? data = await manager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();

        Data = data;
        string? normalizedPhoneNumber = null;
        //验证移动电话号码或电子邮件地址是否已被注册？
        if (PhoneNumber != null)
        {
            try
            {
                normalizedPhoneNumber = new MobilePhoneNumber(PhoneNumber).ToString();
            }
            catch
            {
                ModelState.AddModelError(nameof(PhoneNumber), "Invalid phone number.");
            }
        }

        if (!ModelState.IsValid)
            return Page();

        if (normalizedPhoneNumber != null && manager.Users.Any(p => p.Id != anchor && p.PhoneNumber == normalizedPhoneNumber))
            ModelState.AddModelError("", "移动电话号码已被注册。");
        if (Email != null && manager.Users.Any(p => p.Id != anchor && p.Email == Email))
            ModelState.AddModelError("", "电子邮件地址已被注册。");

        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        Result = await naturalPersonService.SetEmailAsync(Data, Email);
        Result = await naturalPersonService.SetPhoneNumberAsync(Data, normalizedPhoneNumber);

        return Page();
    }
}