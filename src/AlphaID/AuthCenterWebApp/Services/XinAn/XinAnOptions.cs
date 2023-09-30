using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace AuthCenterWebApp.Services.XinAn;

public class XinAnOptions : OAuthOptions
{
    public XinAnOptions()
    {
        this.CallbackPath = "/signin-xinan";
        this.AuthorizationEndpoint = "https://netauth-8088.changingsoft.com/ssoserver/moc2/authorize";
        this.TokenEndpoint = "https://netauth-8088.changingsoft.com/ssoserver/moc2/token";
        this.UserInformationEndpoint = "https://netauth-8088.changingsoft.com/ssoserver/moc2/me";
        this.UserDetailInformationEndpoint = "https://netauth-8088.changingsoft.com/ssoserver/user/info";
        this.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "loginName");
        this.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
        this.ClaimActions.MapJsonKey(ClaimTypes.MobilePhone, "mobile");
        this.ClaimActions.MapJsonKey("openid", "openid");
    }

    public string UserDetailInformationEndpoint { get; set; }
}
