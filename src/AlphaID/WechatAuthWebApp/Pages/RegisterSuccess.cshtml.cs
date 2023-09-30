using Microsoft.AspNetCore.Mvc.RazorPages;
using WechatWebLogin;

namespace WechatAuthWebApp.Pages;

public class RegisterSuccessModel : PageModel
{
    private readonly WechatLoginSessionManager manager;

    public RegisterSuccessModel(WechatLoginSessionManager manager)
    {
        this.manager = manager;
    }

    public string CallbackUri { get; set; } = default!;

    public async Task OnGetAsync(string sessionId)
    {
        this.CallbackUri = await this.manager.BuildCallBackUriAsync(sessionId);
    }
}
