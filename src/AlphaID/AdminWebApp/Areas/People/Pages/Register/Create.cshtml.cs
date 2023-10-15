using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Register;

[IgnoreAntiforgeryToken]
public class CreateModel : PageModel
{
    private readonly ChinesePersonNamePinyinConverter pinyinConverter;
    private readonly NaturalPersonManager manager;

    public CreateModel(ChinesePersonNamePinyinConverter pinyinConverter, NaturalPersonManager manager)
    {
        this.pinyinConverter = pinyinConverter;
        this.manager = manager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (MobilePhoneNumber.TryParse(this.Input.Mobile, out var phoneNumber))
        {
            this.ModelState.AddModelError("", "�ƶ��绰������Ч��");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        var userName = this.Input.Email ?? phoneNumber.PhoneNumber;


        var builder = new PersonBuilder(userName);
        builder.SetMobile(phoneNumber);

        var (phoneticSurname, phoneticGivenName) = this.pinyinConverter.Convert(this.Input.Surname, this.Input.GivenName);
        var chinesePersonName = new ChinesePersonName(this.Input.Surname, this.Input.GivenName, phoneticSurname, phoneticGivenName);

        builder.UseChinesePersonName(chinesePersonName);

        var person = builder.Person;

        var result = await this.manager.CreateAsync(person);

        if (result.Succeeded)
            return this.RedirectToPage("../Detail/Index", new { id = person.Id });

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError("", error.Description);
        }
        return this.Page();
    }

    /// <summary>
    /// ����ƶ��绰����Ч�Ժ�Ψһ�ԡ�
    /// </summary>
    /// <param name="mobile"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostCheckMobileAsync(string mobile)
    {
        if (string.IsNullOrEmpty(mobile))
            return new JsonResult(true);

        if (!MobilePhoneNumber.TryParse(mobile, out MobilePhoneNumber mobilePhoneNumber))
            return new JsonResult($"�ƶ��绰������Ч");

        var person = await this.manager.FindByMobileAsync(mobilePhoneNumber.ToString());
        return person != null ? new JsonResult("���ƶ��绰��ע��") : (IActionResult)new JsonResult(true);
    }

    /// <summary>
    /// ��ȡƴ����
    /// </summary>
    /// <param name="charactors"></param>
    /// <returns></returns>
    public IActionResult OnGetPinyin(string surname, string givenName)
    {
        if (string.IsNullOrWhiteSpace(givenName))
            return this.Content(string.Empty);
        var (phoneticSurname, phoneticGivenName) = this.pinyinConverter.Convert(surname, givenName);
        var chinesePersonName = new ChinesePersonName(surname, givenName, phoneticSurname, phoneticGivenName);
        return this.Content($"{chinesePersonName.PhoneticSurname} {chinesePersonName.PhoneticGivenName}".Trim());
    }

    public class InputModel
    {
        [Display(Name = "Surname")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string Surname { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Given name")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Validate_StringLength")]
        public string GivenName { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; } = default!;

        public string PhoneticSurname { get; set; } = default!;

        public string PhoneticGivenName { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Phonetic display name")]
        public string PhoneticDisplayName { get; set; } = default!;

        [Display(Name = "Gender")]
        public Sex Sex { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Mobile phone number")]
        [PageRemote(PageHandler = "CheckMobile", HttpMethod = "post", AdditionalFields = "__RequestVerificationToken")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "Validate_StringLength")]
        public string Mobile { get; set; } = default!;

        public string? Email { get; set; }
    }
}
