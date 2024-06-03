using Duende.IdentityServer.Models;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public static class ClientConstants
{
    public static readonly List<GrantTypeItem> GrantTypes =
    [
        new GrantTypeItem(GrantType.Implicit, "隐式"),
        new GrantTypeItem(GrantType.AuthorizationCode, "授权码"),
        new GrantTypeItem(GrantType.ClientCredentials, "客户端凭证"),
        new GrantTypeItem(GrantType.ResourceOwnerPassword, "资源所有者密码"),
        new GrantTypeItem(GrantType.DeviceFlow, "设备码"),
        new GrantTypeItem(GrantType.Hybrid, "混合")
    ];
}
