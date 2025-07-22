using AlphaIdPlatform.Platform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ForgetPasswordModel(IServiceProvider serviceProvider) : PageModel
{
    public bool SupportVerificationCodeService => serviceProvider.GetService<IVerificationCodeService>() is not null;

    public void OnGet()
    {
    }
}