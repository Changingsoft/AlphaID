using AlphaIDPlatform;
using AlphaIDPlatform.Platform;
using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class FindPasswordByEmailModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly IEmailSender emailSender;
    private readonly ProductInfo production;

    public FindPasswordByEmailModel(IEmailSender emailSender, NaturalPersonManager userManager, IOptions<ProductInfo> production)
    {
        this.emailSender = emailSender;
        this.userManager = userManager;
        this.production = production.Value;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByEmailAsync(this.Input.Email);
            if (user == null || !await this.userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return this.RedirectToPage("FindPasswordByEmailConfirmation");
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = this.Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { code },
                protocol: this.Request.Scheme);

            await this.emailSender.SendEmailAsync(
                this.Input.Email,
                $"����{this.production.Name}�˻�����",
                $"<p>��������ͨ���ʼ�����{this.production.Name}���롣����<a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>����</a>����ʼ�������롣</p>" +
                $"<p>���޷�������ӣ��뽫�������Ӹ��ƣ������������ճ�����򿪣�</p>" +
                $"<p><span>{HtmlEncoder.Default.Encode(callbackUrl!)}<span></p>" +
                $"<p></p>" +
                $"<p>{this.production.Name}�Ŷ�</p>");

            return this.RedirectToPage("FindPasswordByEmailConfirmation");
        }

        return this.Page();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;
    }
}
