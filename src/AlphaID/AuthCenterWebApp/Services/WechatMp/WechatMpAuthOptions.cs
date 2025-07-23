using Microsoft.AspNetCore.Authentication.OAuth;

namespace AuthCenterWebApp.Services.WechatMp;

public class WechatMpAuthOptions : OAuthOptions
{
    public WechatMpAuthOptions()
    {
        CallbackPath = "/signin-wechat-mp";
        AuthorizationEndpoint = "https://open.weixin.qq.com/connect/oauth2/authorize";
        TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";
        Scope.Clear();
        Scope.Add("snsapi_base");
        StateDataFormat = new ShortIdStateDataFormat();
    }
}