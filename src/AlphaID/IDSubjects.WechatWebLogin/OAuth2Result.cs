using Newtonsoft.Json;

namespace IDSubjects.WechatWebLogin;

/// <summary>
/// OAuth2结果。
/// </summary>
public class OAuth2Result
{
    /// <summary>
    /// Access Token.
    /// </summary>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = default!;

    /// <summary>
    /// Token Type。
    /// </summary>
    [JsonProperty("token_type")]
    public string TokenType { get; set; } = default!;

    /// <summary>
    /// Expires in seconds.
    /// </summary>
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 要访问的资源标识符。
    /// </summary>
    public string? Resource { get; set; }

    /// <summary>
    /// 刷新令牌。
    /// </summary>
    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 刷新令牌的生存期（秒）
    /// </summary>
    [JsonProperty("refresh_token_expires_in")]
    public int? RefreshTokenExpiresIn { get; set; }

    /// <summary>
    /// OpenID Connect 令牌。
    /// </summary>
    [JsonProperty("id_token")]
    public string IdToken { get; set; } = default!;
}
