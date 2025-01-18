using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using AlphaIdPlatform;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class FindPasswordByEmailModel(
    IEmailSender emailSender,
    UserManager<NaturalPerson> userManager,
    IOptions<ProductInfo> production) : PageModel
{
    private readonly ProductInfo _production = production.Value;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            NaturalPerson? user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null || !await userManager.IsEmailConfirmedAsync(user))
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("FindPasswordByEmailConfirmation");

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string? callbackUrl = Url.Page(
                "/Account/ResetPassword",
                null,
                new { code },
                Request.Scheme);

            await emailSender.SendEmailAsync(
                Input.Email,
                $"重置{_production.Name}账户密码",
                $"<p>您已请求通过邮件重设{_production.Name}密码。请点击<a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>这里</a>，开始重设密码。</p>" +
                $"<p>若无法点击链接，请将下列链接复制，并在浏览器中粘贴并打开：</p>" +
                $"<p><span>{HtmlEncoder.Default.Encode(callbackUrl!)}<span></p>" +
                $"<p></p>" +
                $"<p>{_production.Name}团队</p>");

            return RedirectToPage("FindPasswordByEmailConfirmation");
        }

        return Page();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;
    }
}