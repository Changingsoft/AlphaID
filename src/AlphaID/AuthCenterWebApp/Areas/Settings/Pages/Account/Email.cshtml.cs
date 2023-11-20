#nullable disable

using AlphaIdPlatform;
using AlphaIdPlatform.Platform;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class EmailModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly IEmailSender emailSender;
    private readonly ProductInfo production;

    public EmailModel(
        NaturalPersonManager userManager,
        IEmailSender emailSender,
        IOptions<ProductInfo> production)
    {
        this.userManager = userManager;
        this.emailSender = emailSender;
        this.production = production.Value;
    }

    [Display(Name = "Email")]
    public string Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }

    private async Task LoadAsync(NaturalPerson user)
    {
        var email = await this.userManager.GetEmailAsync(user);
        this.Email = email;

        this.Input = new InputModel
        {
            NewEmail = email,
        };

        this.IsEmailConfirmed = await this.userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        await this.LoadAsync(user);
        return this.Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        if (!this.ModelState.IsValid)
        {
            await this.LoadAsync(user);
            return this.Page();
        }

        var email = await this.userManager.GetEmailAsync(user);
        if (this.Input.NewEmail != email)
        {
            var userId = await this.userManager.GetUserIdAsync(user);
            var code = await this.userManager.GenerateChangeEmailTokenAsync(user, this.Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = this.Url.Page(
                "/Account/ConfirmEmailChange",
                pageHandler: null,
                values: new { area = "", userId, email = this.Input.NewEmail, code },
                protocol: this.Request.Scheme);
            await this.emailSender.SendEmailAsync(
                this.Input.NewEmail,
                "确认您的邮件地址",
                $"<p>您已请求更改电子邮件地址，请单击<a href='{callbackUrl}'>这里</a>以确认您的邮件地址。</p>" +
                $"<p>{this.production.Name}团队</p>");

            this.StatusMessage = "变更电子邮件的确认链接已发至您的新邮箱，请到您的邮箱查收邮件并完成变更确认。";
            return this.RedirectToPage();
        }

        this.StatusMessage = "您的邮件地址未更改。";
        return this.RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        if (!this.ModelState.IsValid)
        {
            await this.LoadAsync(user);
            return this.Page();
        }

        var userId = await this.userManager.GetUserIdAsync(user);
        var email = await this.userManager.GetEmailAsync(user);
        var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = this.Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId, code },
            protocol: this.Request.Scheme);
        await this.emailSender.SendEmailAsync(
            email,
            "确认您的邮件地址",
            $"<p>您已请求更改电子邮件地址，请单击<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>这里</a>以确认您的邮件地址。</p>" +
            $"<p>{this.production.Name}团队</p>");

        this.StatusMessage = "验证邮件已发送，请到您的邮箱检查邮件。";
        return this.RedirectToPage();
    }
}
