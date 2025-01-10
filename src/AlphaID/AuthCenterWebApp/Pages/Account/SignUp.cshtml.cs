using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AlphaIdPlatform;
using AlphaIdPlatform.Platform;
using BotDetect.Web.Mvc;
using Duende.IdentityServer;
using IdentityModel;
using IdSubjects;
using IdSubjects.ChineseName;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace AuthCenterWebApp.Pages.Account;

//[SecurityHeaders]
[AllowAnonymous]
public class SignUpModel(
    NaturalPersonManager naturalPersonManager,
    IVerificationCodeService verificationCodeService,
    ChinesePersonNamePinyinConverter chinesePersonNamePinyinConverter,
    IOptions<ProductInfo> production,
    SignInManager<NaturalPerson> signInManager,
    IStringLocalizer<SignUpModel> stringLocalizer) : PageModel
{
    private readonly ProductInfo _production = production.Value;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty]
    [Display(Name = "User name")]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
    public string? UserName { get; set; }

    public string? ExternalLoginMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? external)
    {
        AuthenticateResult externalAuthResult =
            await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (!externalAuthResult.Succeeded) return Page();

        ClaimsPrincipal? externalPrincipal = externalAuthResult.Principal;

        UserName = externalPrincipal.FindFirstValue(JwtClaimTypes.PreferredUserName);
        Input = new InputModel
        {
            Email = externalPrincipal.FindFirstValue(JwtClaimTypes.Email) ??
                    externalPrincipal.FindFirstValue(ClaimTypes.Email) ??
                    externalPrincipal.FindFirstValue(ClaimTypes.Upn),
            GivenName = externalPrincipal.FindFirstValue(JwtClaimTypes.GivenName) ??
                        externalPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? "",
            Surname = externalPrincipal.FindFirstValue(JwtClaimTypes.FamilyName) ??
                      externalPrincipal.FindFirstValue(ClaimTypes.Surname) ?? ""
        };
        string? mobile = externalPrincipal.FindFirstValue(ClaimTypes.MobilePhone)
                         ?? externalPrincipal.FindFirstValue(JwtClaimTypes.PhoneNumber);
        if (MobilePhoneNumber.TryParse(mobile, out MobilePhoneNumber phoneNumber))
            Input.Mobile = phoneNumber.PhoneNumber;
        if (Enum.TryParse(
                externalPrincipal.FindFirstValue(JwtClaimTypes.Gender) ??
                externalPrincipal.FindFirstValue(ClaimTypes.Gender), out Gender result)) Input.Sex = result;
        if (DateOnly.TryParse(
                externalPrincipal.FindFirstValue(JwtClaimTypes.BirthDate) ??
                externalPrincipal.FindFirstValue(ClaimTypes.DateOfBirth), out DateOnly dateOfBirth))
            Input.DateOfBirth = dateOfBirth;
        ExternalLoginMessage = $"您正在从外部登录并创建{_production.Name}，请补全相关内容以创建{_production.Name}。";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Input.Agree)
        {
            ModelState.AddModelError("Input.Agree", $"您必须了解并同意服务协议，才能继续创建{_production.Name}。");
            return Page();
        }

        if (!MobilePhoneNumber.TryParse(Input.Mobile, out MobilePhoneNumber phoneNumber))
            ModelState.AddModelError("Input.PhoneNumber", stringLocalizer["Invalid mobile phone number."]);
        if (!ModelState.IsValid)
            return Page();

        if (!await verificationCodeService.VerifyAsync(phoneNumber.ToString(), Input.VerificationCode))
            ModelState.AddModelError("Input.VerificationCode", "验证码无效");
        if (!ModelState.IsValid)
            return Page();

        //如果来自外埠登录？
        AuthenticateResult externalLoginResult =
            await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);


        (string pinyinSurname, string pinyinGivenName) =
            chinesePersonNamePinyinConverter.Convert(Input.Surname, Input.GivenName);
        var chinesePersonName = new ChinesePersonName(Input.Surname, Input.GivenName, pinyinSurname, pinyinGivenName);
        string userName = Input.Email ?? phoneNumber.PhoneNumber;
        var personBuilder = new PersonBuilder(userName,
            new PersonNameInfo(chinesePersonName.FullName, chinesePersonName.Surname, chinesePersonName.GivenName));
        personBuilder.SetMobile(phoneNumber, true);
        personBuilder.UseChinesePersonName(chinesePersonName);
        if (Input.Email != null)
            personBuilder.SetEmail(Input.Email);

        NaturalPerson person = personBuilder.Build();

        person.DateOfBirth = Input.DateOfBirth;
        person.Gender = Input.Sex;

        IdentityResult result = await naturalPersonManager.CreateAsync(person, Input.NewPassword);
        if (result.Succeeded)
        {
            if (externalLoginResult.Succeeded)
            {
                //Create external login for user.
                Claim userIdClaim = externalLoginResult.Principal.FindFirst(JwtClaimTypes.Subject) ??
                                    externalLoginResult.Principal.FindFirst(ClaimTypes.NameIdentifier) ??
                                    throw new Exception("Unknown userid");
                await naturalPersonManager.AddLoginAsync(person,
                    new UserLoginInfo(externalLoginResult.Properties.Items[".AuthScheme"]!, userIdClaim.Value,
                        externalLoginResult.Properties.Items["schemeDisplayName"]));
            }

            //login user. redirect to user profile center.
            await signInManager.SignInAsync(person, false);
            return RedirectToPage("SignUpSuccess");
        }

        foreach (IdentityError error in result.Errors) ModelState.AddModelError("", error.Description);
        return Page();
    }

    public async Task<IActionResult> OnPostSendVerificationCodeAsync(string mobile)
    {
        if (!MobilePhoneNumber.TryParse(mobile, out MobilePhoneNumber phoneNumber)) return new JsonResult("移动电话号码无效。");
        await verificationCodeService.SendAsync(phoneNumber.ToString());
        return new JsonResult(true);
    }

    public class InputModel
    {
        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "Validate_StringLength")]
        public string Mobile { get; set; } = null!;

        [Display(Name = "Verification code", Prompt = "Received from mobile phone short message.")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        public string VerificationCode { get; set; } = null!;

        [Display(Name = "Surname", Prompt = "Surname")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string Surname { get; set; } = null!;

        [Display(Name = "Given name", Prompt = "Given name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string GivenName { get; set; } = null!;

        [Display(Name = "Gender")]
        public Gender? Sex { get; set; }

        [Display(Name = "Birth date")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }

        [Display(Name = "New password")]
        [Required(ErrorMessage = "Validate_Required")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Validate_StringLength")]
        public string NewPassword { get; set; } = null!;

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Validate_Required")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Validate_StringLength")]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Email (Optional)", Prompt = "someone@examples.com")]
        [EmailAddress(ErrorMessage = "Validate_EmailAddress")]
        public string? Email { get; set; }

        [Display(Name = "Captcha code")]
        [Required(ErrorMessage = "Validate_Required")]
        [CaptchaModelStateValidation("LoginCaptcha", ErrorMessage = "Captcha_Invalid")]
        public string CaptchaCode { get; set; } = null!;

        [Display(Name = "Agree the")]
        public bool Agree { get; set; }
    }
}