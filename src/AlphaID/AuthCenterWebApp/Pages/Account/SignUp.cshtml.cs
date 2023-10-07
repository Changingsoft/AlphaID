using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using BotDetect.Web.Mvc;
using IdentityModel;
using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
#nullable enable
namespace AuthCenterWebApp.Pages.Account;

//[SecurityHeaders]
[AllowAnonymous]
public class SignUpModel : PageModel
{
    private readonly NaturalPersonManager naturalPersonManager;
    private readonly IVerificationCodeService verificationCodeService;
    private readonly ChinesePersonNamePinyinConverter chinesePersonNamePinyinConverter;
    private readonly ProductInfo production;
    private readonly SignInManager<NaturalPerson> signInManager;

    public SignUpModel(NaturalPersonManager naturalPersonManager,
                       IVerificationCodeService verificationCodeService,
                       ChinesePersonNamePinyinConverter chinesePersonNamePinyinConverter,
                       IOptions<ProductInfo> production,
                       SignInManager<NaturalPerson> signInManager)
    {
        this.naturalPersonManager = naturalPersonManager;
        this.verificationCodeService = verificationCodeService;
        this.chinesePersonNamePinyinConverter = chinesePersonNamePinyinConverter;
        this.production = production.Value;
        this.signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public string? ExternalLoginMessage { get; set; } = default!;

    public async Task<IActionResult> OnGet(string? external)
    {
        var externalAuthResult = await this.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!externalAuthResult.Succeeded)
        {
            return this.Page();
        }

        var externalPrincipal = externalAuthResult.Principal;


        this.Input = new InputModel
        {
            Email = externalPrincipal.FindFirstValue(JwtClaimTypes.Email) ?? externalPrincipal.FindFirstValue(ClaimTypes.Email) ?? externalPrincipal.FindFirstValue(ClaimTypes.Upn),
            GivenName = externalPrincipal.FindFirstValue(JwtClaimTypes.GivenName) ?? externalPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? "",
            Surname = externalPrincipal.FindFirstValue(JwtClaimTypes.FamilyName) ?? externalPrincipal.FindFirstValue(ClaimTypes.Surname) ?? ""
        };
        var mobile = externalPrincipal.FindFirstValue(ClaimTypes.MobilePhone)
                     ?? externalPrincipal.FindFirstValue(JwtClaimTypes.PhoneNumber);
        if (MobilePhoneNumber.TryParse(mobile, out var phoneNumber))
        {
            this.Input.Mobile = phoneNumber.PhoneNumber;
        }
        if (Enum.TryParse(externalPrincipal.FindFirstValue(JwtClaimTypes.Gender) ?? externalPrincipal.FindFirstValue(ClaimTypes.Gender), out Sex result))
        {
            this.Input.Sex = result;
        }
        if (DateTime.TryParse(externalPrincipal.FindFirstValue(JwtClaimTypes.BirthDate) ?? externalPrincipal.FindFirstValue(ClaimTypes.DateOfBirth), out var dateOfBirth))
        {
            this.Input.DateOfBirth = dateOfBirth;
        }
        this.ExternalLoginMessage = $"您正在从外部登录并创建{this.production.Name}，请补全相关内容以创建{this.production.Name}。";
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {

        if (!this.Input.Agree)
        {
            this.ModelState.AddModelError("Input.Agree", $"您必须了解并同意服务协议，才能继续创建{this.production.Name}。");
            return this.Page();
        }

        if (!MobilePhoneNumber.TryParse(this.Input.Mobile, out var phoneNumber))
        {
            this.ModelState.AddModelError("Input.Mobile", "移动电话号码格式错误");
        }
        if (!this.ModelState.IsValid)
            return this.Page();

        if (!await this.verificationCodeService.VerifyAsync(phoneNumber.ToString(), this.Input.VerificationCode))
        {
            this.ModelState.AddModelError("Input.VerificationCode", "验证码无效");
        }
        if (!this.ModelState.IsValid)
            return this.Page();

        //如果来自外埠登录？
        var externalLoginResult = await this.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);



        var (pinyinSurname, pinyinGivenName) = this.chinesePersonNamePinyinConverter.Convert(this.Input.Surname, this.Input.GivenName);
        var chinesePersonName = new ChinesePersonName(this.Input.Surname, this.Input.GivenName, pinyinSurname, pinyinGivenName);
        var userName = this.Input.Email ?? phoneNumber.PhoneNumber;
        var personBuilder = new PersonBuilder(userName);
        personBuilder.SetMobile(phoneNumber, true);
        personBuilder.UseChinesePersonName(chinesePersonName);
        if (this.Input.Email != null)
            personBuilder.SetEmail(this.Input.Email);

        var person = personBuilder.Person;
        person.PasswordLastSet = DateTime.Now;
        person.DateOfBirth = this.Input.DateOfBirth;
        person.Sex = this.Input.Sex;

        var result = await this.naturalPersonManager.CreateAsync(person, this.Input.NewPassword);
        if (result.Succeeded)
        {
            if (externalLoginResult.Succeeded)
            {
                //Create external login for user.
                var userIdClaim = externalLoginResult.Principal.FindFirst(JwtClaimTypes.Subject) ??
                      externalLoginResult.Principal.FindFirst(ClaimTypes.NameIdentifier) ??
                      throw new Exception("Unknown userid");
                await this.naturalPersonManager.AddLoginAsync(person, new UserLoginInfo(externalLoginResult.Properties.Items[".AuthScheme"]!, userIdClaim.Value, externalLoginResult.Properties.Items["schemeDisplayName"]));
            }

            //login user. redirect to user profile center.
            await this.signInManager.SignInAsync(person, false);
            return this.RedirectToPage("SignUpSuccess");
        }
        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError("", error.Description);
        }
        return this.Page();
    }

    public async Task<IActionResult> OnPostSendVerificationCodeAsync(string mobile)
    {
        if (!MobilePhoneNumber.TryParse(mobile, out var phoneNumber))
        {
            return new JsonResult("移动电话号码无效。");
        }
        await this.verificationCodeService.SendAsync(phoneNumber.ToString());
        return new JsonResult(true);
    }

    public class InputModel
    {
        [Display(Name = "Mobile phone number", Prompt = "1xxxxxxxxxx")]
        [Required(ErrorMessage = "{0}是必需的。")]
        [StringLength(14, MinimumLength = 11)]
        public string Mobile { get; set; } = default!;

        [Display(Name = "Verification code", Prompt = "Received from mobile phone short message.")]
        [Required(ErrorMessage = "{0}是必需的。")]
        [StringLength(8, MinimumLength = 4)]
        public string VerificationCode { get; set; } = default!;

        [Display(Name = "Surname", Prompt = "Surname")]
        [Required(ErrorMessage = "{0}是必需的。")]
        [StringLength(10)]
        public string Surname { get; set; } = default!;

        [Display(Name = "Given name", Prompt = "Given name")]
        [Required(ErrorMessage = "{0}是必需的。")]
        [StringLength(10)]
        public string GivenName { get; set; } = default!;

        [Display(Name = "Gender")]
        public Sex? Sex { get; set; }

        [Display(Name = "Birth date")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "New password")]
        [Required(ErrorMessage = "{0}是必需的。")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 6)]
        public string NewPassword { get; set; } = default!;

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "{0}是必需的。")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "新密码和可选密码不一致")]
        [StringLength(32, MinimumLength = 6)]
        public string ConfirmPassword { get; set; } = default!;

        [Display(Name = "Email (Optional)", Prompt = "someone@examples.com")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "键入图片上看到的字符")]
        [CaptchaModelStateValidation("LoginCaptcha", ErrorMessage = "验证无效，请重新尝试")]
        public string CaptchaCode { get; set; } = default!;

        [Display(Name = "了解并同意")]
        public bool Agree { get; set; }
    }
}
