using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WechatWebLogin;

namespace WechatAuthWebApp.Pages;

public class WechatLoginModel : PageModel
{
    private readonly WechatLoginSessionManager sessionManager;

    public WechatLoginModel(WechatLoginSessionManager sessionManager)
    {
        this.sessionManager = sessionManager;
    }

    public List<string> ErrorMessages { get; set; } = new List<string>();

    /// <summary>
    /// 向微信发起授权后，微信通过Redirect重定向到该地址，并携带授权码和state。
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnGet(string code, string state)
    {
        try
        {
            var session = await this.sessionManager.WechatLoginAsync(state, code);
            //通过微信OpenId查找绑定的登录标识。
            if (session.WechatUser == null)
            {
                //没有找到绑定信息，跳转到绑定阶段。
                return this.RedirectToPage("/SignIn", new { sessionid = session.Id });
            }

            return this.Redirect(await this.sessionManager.BuildCallBackUriAsync(session.Id));
        }
        catch (Exception ex)
        {
            this.ErrorMessages.Add(ex.Message);
            return this.Page();
        }
    }
}
