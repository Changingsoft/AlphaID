using AlphaIdPlatform;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using AuthCenterWebApp.CaptchaValidation;
using ChineseName;
using Duende.IdentityModel;
using Duende.IdentityServer;
using IdSubjects;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account;

//[SecurityHeaders]
[AllowAnonymous]
public class SignUpModel(
    NaturalPersonService naturalPersonService,
    UserManager<NaturalPerson> applicationUserManager,
    IServiceProvider serviceProvider,
    ChinesePersonNamePinyinConverter chinesePersonNamePinyinConverter,
    IOptions<ProductInfo> production,
    SignInManager<NaturalPerson> signInManager,
    IStringLocalizer<SignUpModel> stringLocalizer,
    ICaptcha captcha) : PageModel
{
    private readonly ProductInfo _production = production.Value;

    /// <summary>
    /// 本页图形验证码实例 ID，与 <c>/captcha?id=</c> 及验证码图片元素的 <c>data-captcha-id</c> 一致。
    /// </summary>
    private const string CaptchaId = "SignUp";

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty]
    [Display(Name = "User name")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
    public string? UserName { get; set; }

    public string? ExternalLoginMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Verification code")]
    [DataType(DataType.Password)]
    public string? VerificationCode { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [Display(Name = "Captcha code")]
    public string CaptchaCode { get; set; } = null!;
    public IVerificationCodeService? VerificationCodeService => serviceProvider.GetService<IVerificationCodeService>();

    public async Task<IActionResult> OnGet()
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
            PhoneNumber = phoneNumber.PhoneNumber;
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

    public async Task<IActionResult> OnPost()
    {
        //图形验证码校验。放在最前面，确保任何 return Page() 之前验证码都已被消费，
        //以维持视图 _CaptchaScripts 中「页面重新渲染即换图」的约定。
        if (!CaptchaValidator.Validate(captcha, CaptchaId, CaptchaCode))
            ModelState.AddModelError(nameof(CaptchaCode), Resources.SharedResource.Captcha_Invalid);

        if (!Input.Agree)
        {
            ModelState.AddModelError("Input.Agree", $"您必须了解并同意服务协议，才能继续创建{_production.Name}。");
            return Page();
        }

        if (!MobilePhoneNumber.TryParse(PhoneNumber, out MobilePhoneNumber phoneNumber))
            ModelState.AddModelError("Input.PhoneNumber", stringLocalizer["Invalid mobile phone number."]);

        if (!ModelState.IsValid)
            return Page();

        var phoneNumberConfirmed = false;
        if (VerificationCodeService is not null)
        {
            if (!await VerificationCodeService.VerifyAsync(phoneNumber.ToString(), VerificationCode!))
            {
                ModelState.AddModelError("Input.VerificationCode", "验证码无效");
            }
            else
            {
                phoneNumberConfirmed = true;
            }
        }

        if (!ModelState.IsValid)
            return Page();

        //如果来自外埠登录？
        AuthenticateResult externalLoginResult =
            await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);


        (string pinyinSurname, string pinyinGivenName) =
            chinesePersonNamePinyinConverter.Convert(Input.Surname, Input.GivenName);
        var chinesePersonName = new ChinesePersonName(Input.Surname, Input.GivenName, pinyinSurname, pinyinGivenName);
        string userName = Input.Email ?? phoneNumber.PhoneNumber;

        var person = new NaturalPerson(userName)
        {
            Email = Input.Email,
            PhoneNumber = phoneNumber.ToString(),
            PhoneNumberConfirmed = phoneNumberConfirmed,
            FamilyName = chinesePersonName.Surname,
            GivenName = chinesePersonName.GivenName,
            Name = chinesePersonName.FullName,
            PhoneticSurname = chinesePersonName.PhoneticSurname,
            PhoneticGivenName = chinesePersonName.PhoneticGivenName,
            SearchHint = chinesePersonName.PhoneticName,
            DateOfBirth = Input.DateOfBirth,
            Gender = Input.Sex
        };

        IdentityResult result = await naturalPersonService.CreateAsync(person, Input.NewPassword);
        if (result.Succeeded)
        {
            if (externalLoginResult.Succeeded)
            {
                //Create external login for user.
                Claim userIdClaim = externalLoginResult.Principal.FindFirst(JwtClaimTypes.Subject) ??
                                    externalLoginResult.Principal.FindFirst(ClaimTypes.NameIdentifier) ??
                                    throw new Exception("Unknown userid");
                await applicationUserManager.AddLoginAsync(person,
                    new UserLoginInfo(externalLoginResult.Properties.Items[".AuthScheme"]!, userIdClaim.Value,
                        externalLoginResult.Properties.Items["schemeDisplayName"]));
            }

            //login user. redirect to user profile center.
            await signInManager.SignInAsync(person, false);
            return RedirectToPage("SignUpSuccess");
        }

        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return Page();
    }

    /// <summary>
    /// 发送短信验证码。图形验证码校验成功时保留验证码，使随后的表单提交仍能校验通过；
    /// 校验失败时消费验证码，并由前端据 <c>captchaFailed</c> 刷新验证码图片。
    /// </summary>
    public async Task<IActionResult> OnPostSendVerificationCode()
    {
        if (!CaptchaValidator.ValidateKeepingOnSuccess(captcha, CaptchaId, CaptchaCode))
            return new JsonResult(new { ok = false, captchaFailed = true, message = Resources.SharedResource.Captcha_Invalid });

        if (!MobilePhoneNumber.TryParse(PhoneNumber, out MobilePhoneNumber phoneNumber))
            return new JsonResult(new { ok = false, captchaFailed = false, message = Resources.SharedResource.PhoneNumberInvalid });

        await VerificationCodeService!.SendAsync(phoneNumber.ToString());
        return new JsonResult(new { ok = true, captchaFailed = false, message = (string?)null });
    }

    public class InputModel
    {

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

        [Display(Name = "Email", Prompt = "someone@examples.com")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Validate_EmailFormat")]
        public string? Email { get; set; }

        [Display(Name = "Agree the")]
        public bool Agree { get; set; }
    }
}