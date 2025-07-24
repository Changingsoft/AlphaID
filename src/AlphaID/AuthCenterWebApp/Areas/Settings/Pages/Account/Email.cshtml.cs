using AlphaIdPlatform;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class EmailModel(
    UserManager<NaturalPerson> userManager,
    IEmailSender emailSender,
    IOptions<ProductInfo> production) : PageModel
{
    private readonly ProductInfo _production = production.Value;

    [Display(Name = "Email")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Validate_EmailFormat")]
    public string? Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    private async Task LoadAsync(NaturalPerson user)
    {
        string? email = await userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email ?? "",
        };

        IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        string? email = await userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            string userId = await userManager.GetUserIdAsync(user);
            string code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string? callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                null,
                new { area = "", userId, email = Input.NewEmail, code },
                Request.Scheme);
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
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        string userId = await userManager.GetUserIdAsync(user);
        string? email = await userManager.GetEmailAsync(user);
        if (email == null)
        {
            StatusMessage = "没有设置邮件地址";
            return RedirectToPage();
        }
        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string? callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            null,
            new { area = "", userId, code },
            Request.Scheme);
        Debug.Assert(callbackUrl != null);
        await emailSender.SendEmailAsync(
            email,
            "确认邮件地址",
            $"<p>您已请求更改电子邮件地址，请单击<a href='{callbackUrl}'>这里</a>以确认您的邮件地址。</p>" +
            $"<p>{_production.Name}团队</p>");

        StatusMessage = "验证邮件已发送，请到您的邮箱检查邮件。";
        return RedirectToPage();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; } = null!;
    }
}