using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WechatWebLogin;

namespace WechatAuthWebApp.Pages;

public class SignInSuccessModel : PageModel
{
    private readonly WechatLoginSessionManager manager;

    public SignInSuccessModel(WechatLoginSessionManager manager)
    {
        this.manager = manager;
    }

    public string FederalAccessToken { get; set; } = default!;

    public string RedirectUri { get; set; } = default!;


    public async Task<IActionResult> OnGetAsync(string sessionId)
    {
        this.RedirectUri = await this.manager.BuildCallBackUriAsync(sessionId);

        return this.Page();
    }
}
