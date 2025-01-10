using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.WechatWebLogin;

/// <summary>
/// </summary>
[Table("WechatLoginSession")]
public class WechatLoginSession
{
    /// <summary>
    /// </summary>
    protected WechatLoginSession()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="wechatAppId"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="resource"></param>
    /// <param name="redirectUri"></param>
    internal WechatLoginSession(string wechatAppId,
        string clientId,
        string clientSecret,
        string resource,
        string redirectUri)
    {
        WechatAppId = wechatAppId;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Resource = resource;
        RedirectUri = redirectUri;
        Id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        WhenExpires = DateTime.UtcNow.AddMinutes(10.0D);
    }

    /// <summary>
    /// </summary>
    [Key]
    public string Id { get; protected set; } = null!;

    /// <summary>
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string WechatAppId { get; protected set; } = null!;


    /// <summary>
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string ClientId { get; protected set; } = null!;

    /// <summary>
    /// </summary>
    [MaxLength(150)]
    [Unicode(false)]
    public string Resource { get; protected set; } = null!;

    /// <summary>
    /// </summary>
    public DateTime WhenExpires { get; protected set; }

    /// <summary>
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string ClientSecret { get; set; } = null!;

    /// <summary>
    /// </summary>
    [MaxLength(150)]
    [Unicode(false)]
    public string RedirectUri { get; set; } = null!;

    /// <summary>
    /// </summary>
    [MaxLength(512)]
    [Unicode(false)]
    public string WechatOAuthToken { get; set; } = null!;

    /// <summary>
    /// </summary>
    [MaxLength(128)]
    [Unicode(false)]
    public string OpenId { get; set; } = null!;

    /// <summary>
    ///     微信AccessToken的过期秒数。
    /// </summary>
    public int WechatOAuthTokenExpiresIn { get; set; } = 0!;

    /// <summary>
    /// </summary>
    public DateTime WechatOauthTokenExpires { get; set; } = default!;

    /// <summary>
    ///     移动电话。
    /// </summary>
    [MaxLength(18)]
    [Unicode(false)]
    public string Mobile { get; set; } = null!;

    /// <summary>
    ///     与此会话关联的微信用户。
    /// </summary>
    [NotMapped]
    public WechatUserIdentifier? WechatUser { get; internal set; } = null!;
}