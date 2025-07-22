using Microsoft.AspNetCore.Authentication;
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

public class ShortIdStateDataFormat : ISecureDataFormat<AuthenticationProperties>
{
    public string Protect(AuthenticationProperties data)
    {
        var stateId = Guid.NewGuid().ToString("N");
        WechatMpOAuthHandler.StateStore.Save(stateId, data);
        return stateId;
    }

    public string Protect(AuthenticationProperties data, string? purpose)
    {
        throw new NotImplementedException();
    }

    public AuthenticationProperties? Unprotect(string? protectedText)
    {
        if (protectedText == null)
            return null;
        return WechatMpOAuthHandler.StateStore.Get(protectedText);
    }

    public AuthenticationProperties? Unprotect(string? protectedText, string? purpose)
    {
        throw new NotImplementedException();
    }
}