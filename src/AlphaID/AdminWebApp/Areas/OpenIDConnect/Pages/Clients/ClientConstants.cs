using Duende.IdentityServer.Models;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public static class ClientConstants
{
    public static readonly List<GrantTypeItem> GrantTypes =
    [
        new(GrantType.Implicit, "隐式"),
        new(GrantType.AuthorizationCode, "授权码"),
        new(GrantType.ClientCredentials, "客户端凭证"),
        new(GrantType.ResourceOwnerPassword, "资源所有者密码"),
        new(GrantType.DeviceFlow, "设备码"),
        new(GrantType.Hybrid, "混合")
    ];
}
