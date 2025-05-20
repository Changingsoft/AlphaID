using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using ChineseName;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Register;

public class CreateModel(ChinesePersonNamePinyinConverter pinyinConverter, UserManager<NaturalPerson> manager) : PageModel
{
    [BindProperty]
    [Display(Name = "User name")]
    [PageRemote(PageHandler = "CheckUserName", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
    [Required(ErrorMessage = "Validate_Required")]
    public string UserName { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Phone number", Description = "仅支持中国大陆手机号")]
    [PageRemote(PageHandler = "CheckMobile", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Validate_StringLength")]
    public string? Mobile { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Email")]
    [PageRemote(PageHandler = "CheckEmail", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
    [StringLength(14, MinimumLength = 11, ErrorMessage = "Validate_StringLength")]
    public string? Email { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        string? normalizedPhoneNumber = null;
        if (Mobile != null)
        {
            try
            {
                normalizedPhoneNumber = new MobilePhoneNumber(Mobile).ToString();
            }
            catch
            {
                ModelState.AddModelError(nameof(Mobile), "Invalid phone number.");
            }
        }

        if (!ModelState.IsValid)
            return Page();

        var person = new NaturalPerson(UserName)
        {
            Email = Email,
            PhoneNumber = normalizedPhoneNumber,
            FamilyName = Input.Surname,
            GivenName = Input.GivenName,
            Name = Input.DisplayName,
            PhoneticSurname = Input.PhoneticSurname,
            PhoneticGivenName = Input.PhoneticGivenName,
            SearchHint = $"{Input.PhoneticSurname}{Input.GivenName}",
            DateOfBirth = Input.DateOfBirth.HasValue
                ? DateOnly.FromDateTime(Input.DateOfBirth.Value)
                : null,
            Gender = Input.Gender
        };

        Result = await manager.CreateAsync(person);

        if (Result.Succeeded)
            return RedirectToPage("../Detail/Index", new { anchor = person.Id });

        return Page();
    }

    /// <summary>
    /// 检查移动电话的有效性和唯一性。
    /// </summary>
    /// <param name="mobile"></param>
    /// <returns></returns>
    public IActionResult OnPostCheckMobile(string mobile)
    {
        if (string.IsNullOrEmpty(mobile))
            return new JsonResult(true);

        if (!MobilePhoneNumber.TryParse(mobile, out MobilePhoneNumber mobilePhoneNumber))
            return new JsonResult("移动电话号码无效");

        if (!manager.Users.Any(p => p.PhoneNumber == mobilePhoneNumber.ToString())) return new JsonResult(true);
        return new JsonResult("此移动电话已注册");
    }

    public IActionResult OnPostCheckEmail(string email)
    {
        if (manager.Users.Any(p => p.Email == email))
            return new JsonResult("Email is exists.");
        return new JsonResult(true);
    }

    public IActionResult OnPostCheckUserName(string userName)
    {
        if (manager.Users.Any(p => p.UserName == userName))
            return new JsonResult("User name is exists.");
        return new JsonResult(true);
    }

    /// <summary>
    /// 获取拼音。
    /// </summary>
    /// <returns></returns>
    public IActionResult OnGetPinyin(string? surname, string? givenName)
    {

        (string phoneticSurname, string phoneticGivenName) = pinyinConverter.Convert(surname, givenName);
        var chinesePersonName = new ChinesePersonName(surname, givenName, phoneticSurname, phoneticGivenName);
        return new JsonResult(new
        {
            phoneticSurname,
            phoneticGivenName,
        });
    }

    public class InputModel
    {
        [Display(Name = "Surname")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string Surname { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Given name")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Validate_StringLength")]
        public string GivenName { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
        public string DisplayName { get; set; } = null!;

        [Display(Name = "Phonetic surname")]
        public string PhoneticSurname { get; set; } = null!;

        [Display(Name = "Phonetic given name")]
        public string PhoneticGivenName { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Phonetic display name")]
        public string PhoneticDisplayName { get; set; } = null!;

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}