using AlphaIDPlatform.Platform;
using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WechatWebLogin;

namespace WechatAuthWebApp.Pages;

public class SignInModel : PageModel
{
    private readonly IVerificationCodeService verificationCodeService;
    private readonly NaturalPersonManager personManager;
    private readonly WechatLoginSessionManager loginSessionManager;

    public SignInModel(IVerificationCodeService verificationCodeService, NaturalPersonManager personManager, WechatLoginSessionManager loginSessionManager)
    {
        this.verificationCodeService = verificationCodeService;
        this.personManager = personManager;
        this.loginSessionManager = loginSessionManager;
    }

    [BindProperty(SupportsGet = true)]
    public string SessionId { get; set; } = default!;

    [BindProperty]
    public SignInFormModel Form { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var session = await this.loginSessionManager.FindAsync(this.SessionId);
        return session == null ? this.BadRequest() : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
            return this.Page();

        var session = await this.loginSessionManager.FindAsync(this.SessionId);
        if (session == null)
            return this.BadRequest();

        if (!MobilePhoneNumber.TryParse(this.Form.Mobile, out MobilePhoneNumber number))
        {
            this.ModelState.AddModelError(nameof(this.Form.Mobile), "手机号格式不正确");
            return this.Page();
        }

        //验证手机号
        if (!await this.verificationCodeService.VerifyAsync(number.ToString(), this.Form.VerificationCode))
        {
            this.ModelState.AddModelError(nameof(this.Form.VerificationCode), "无效的验证码。");
            return this.Page();
        }

        session.Mobile = number.ToString();
        await this.loginSessionManager.UpdateAsync(session);

        //通过手机号查找登录信息
        //若找到，则为该登录进行openid绑定
        //若找不到，则跳转注册
        var personObjectId = await this.personManager.FindByMobileAsync(number.ToString());
        if (personObjectId == default)
        {
            return this.RedirectToPage("/Register", new { sessionid = session.Id, openid = session.OpenId });
        }


        await this.loginSessionManager.BindingPersonAsync(this.SessionId, personObjectId.Id);

        //显示绑定成功消息
        return this.RedirectToPage("/SignInSuccess", new { sessionid = session.Id });
    }

    public async Task OnPostSendVerificationCodeAsync(string mobile)
    {
        await this.verificationCodeService.SendAsync(mobile);
    }

    public class SignInFormModel
    {
        [Display(Name = "PhoneNumber phone number")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(11, ErrorMessage = "Validate_StringLength")]
        [RegularExpression(@"^\d{11}$")]
        public string Mobile { get; set; } = default!;

        [Display(Name = "Verification code")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(6, ErrorMessage = "Validate_StringLength")]
        public string VerificationCode { get; set; } = default!;
    }


}
