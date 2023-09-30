using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WechatWebLogin;

namespace WechatAuthWebApp.Pages;

public class AuthorizeModel : PageModel
{
    private readonly IConfiguration configuration;
    private readonly WechatLoginSessionManager sessionManager;

    public AuthorizeModel(IConfiguration configuration, WechatLoginSessionManager sessionManager)
    {
        this.configuration = configuration;
        this.sessionManager = sessionManager;
    }

    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 微信SPA应用程序将用户重定向到此处，开始启动微信认证。
    /// </summary>
    /// <param name="wxAppId">微信公众号的AppId。</param>
    /// <param name="clientId">登记的客户端Id。</param>
    /// <param name="resource">要访问的资源标识符。</param>
    /// <param name="redirect_uri">验证结束后的回调URI。</param>
    public async Task<IActionResult> OnGet(string wxAppId, string clientId, string resource, string redirect_uri)
    {
        if (string.IsNullOrWhiteSpace(wxAppId))
            this.ErrorMessage += "参数错误：微信appid无效。";

        if (string.IsNullOrWhiteSpace(clientId))
            this.ErrorMessage += "参数错误：clientid无效。";

        if (string.IsNullOrWhiteSpace(redirect_uri))
            this.ErrorMessage += "参数错误：redirect_uri无效。";

        if (!string.IsNullOrEmpty(this.ErrorMessage))
            return this.Page();

        var session = await this.sessionManager.CreateAsync(wxAppId, clientId, resource, redirect_uri); //登记一个会话并将Id放入state，以便回调时做关联。

        var scope = "snsapi_userinfo";
        var callBackUri = new Uri(new Uri(this.configuration["AppBaseAddress"]!), "WechatLogin");
        var queryDict = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"appid",wxAppId },
            {"redirect_uri", callBackUri.ToString() },
            {"response_type", "code" },
            {"scope", scope },
            {"state", session.Id }
        });
        var redirectUri = string.Format(WechatAuthorizeUri, await queryDict.ReadAsStringAsync());

        return this.Redirect(redirectUri);
    }

    private const string WechatAuthorizeUri = "https://open.weixin.qq.com/connect/oauth2/authorize?{0}#wechat_redirect";
}
