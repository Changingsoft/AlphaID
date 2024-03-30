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

public class EmailModel(
    NaturalPersonManager userManager,
    IEmailSender emailSender,
    IOptions<ProductInfo> production) : PageModel
{
    private readonly ProductInfo _production = production.Value;

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
        var email = await userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email,
        };

        IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            var userId = await userManager.GetUserIdAsync(user);
            var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                pageHandler: null,
                values: new { area = "", userId, email = Input.NewEmail, code },
                protocol: Request.Scheme);
            await emailSender.SendEmailAsync(
                Input.NewEmail,
                "确认您的邮件地址",
                $"<p>您已请求更改电子邮件地址，请单击<a href='{callbackUrl}'>这里</a>以确认您的邮件地址。</p>" +
                $"<p>{_production.Name}团队</p>");

            StatusMessage = "变更电子邮件的确认链接已发至您的新邮箱，请到您的邮箱查收邮件并完成变更确认。";
            return RedirectToPage();
        }

        StatusMessage = "您的邮件地址未更改。";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var userId = await userManager.GetUserIdAsync(user);
        var email = await userManager.GetEmailAsync(user);
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId, code },
            protocol: Request.Scheme);
        await emailSender.SendEmailAsync(
            email,
            "确认您的邮件地址",
            $"<p>您已请求更改电子邮件地址，请单击<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>这里</a>以确认您的邮件地址。</p>" +
            $"<p>{_production.Name}团队</p>");

        StatusMessage = "验证邮件已发送，请到您的邮箱检查邮件。";
        return RedirectToPage();
    }
}
