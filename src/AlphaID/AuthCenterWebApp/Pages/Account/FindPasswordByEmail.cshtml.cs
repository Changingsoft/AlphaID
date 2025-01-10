using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using AlphaIdPlatform;
using AlphaIdPlatform.Platform;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class FindPasswordByEmailModel(
    IEmailSender emailSender,
    NaturalPersonManager userManager,
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
                $"����{_production.Name}�˻�����",
                $"<p>��������ͨ���ʼ�����{_production.Name}���롣����<a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>����</a>����ʼ�������롣</p>" +
                $"<p>���޷�������ӣ��뽫�������Ӹ��ƣ������������ճ�����򿪣�</p>" +
                $"<p><span>{HtmlEncoder.Default.Encode(callbackUrl!)}<span></p>" +
                $"<p></p>" +
                $"<p>{_production.Name}�Ŷ�</p>");

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