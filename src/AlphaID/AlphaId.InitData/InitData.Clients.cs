namespace AlphaId.InitData;

public static partial class InitData
{
    /// <summary>
    /// Admin Center 使用的客户端。其 <c>ClientSecret</c> 配置在 <c>AdminWebApp/appsettings.json</c> 的
    /// <c>OidcClient</c> 节，集成测试亦以该客户端发起授权码流程。
    /// </summary>
    public static InitClient ManagementCenter { get; } = new(
        ClientId: "d70700eb-c4d8-4742-a79a-6ecf2064b27c",
        ClientName: "AlphaID Management Center",
        Created: Time("2022-12-23T20:37:00.0000000"),
        Secret: new InitClientSecret(ManagementCenterClientSecret, Time("2022-12-23T20:39:00.0000000")),
        Description: "Used for current web application.",
        GrantTypes: ["authorization_code", "client_credentials", "password"],
        RedirectUris: ["https://localhost:49728/signin-oidc"],
        PostLogoutRedirectUris: ["https://localhost:49728/signout-callback-oidc"],
        Scopes: ["openid", "profile", "public"]);

    /// <summary>
    /// AuthCenter 的 Swagger UI 使用的客户端。其 <c>ClientSecret</c> 配置在
    /// <c>AuthCenterWebApp/appsettings.Development.json</c> 的 <c>Swagger</c> 节。
    /// </summary>
    public static InitClient AuthCenterSwaggerUi { get; } = new(
        ClientId: "43670b09-b161-46ca-b59a-c0fbde526394",
        ClientName: "AlphaID AuthCenter Swagger UI",
        Created: Time("2022-12-23T20:37:00.0000000"),
        Secret: new InitClientSecret(SwaggerUiClientSecret, Time("2022-12-23T20:39:00.0000000")),
        GrantTypes: ["authorization_code"],
        RedirectUris:
        [
            "https://localhost:49726/api-docs/oauth2-redirect.html",
            "https://oauth.pstmn.io/v1/callback",
        ],
        Scopes: ["openid", "profile", "membership"]);

    /// <summary>全部内置客户端。</summary>
    /// <remarks>
    /// <c>AlphaIdWebAPI</c> 使用的客户端（<c>5aa8bed6-4f57-47ac-82e9-08b5874c64e3</c>）**不在此列**：
    /// 它从未出现在历史脚本中，而是通过管理界面创建的，因此属于「部署时配置」而非「内置数据」。
    /// </remarks>
    public static IReadOnlyList<InitClient> Clients { get; } =
    [
        ManagementCenter,
        AuthCenterSwaggerUi,
    ];
}
