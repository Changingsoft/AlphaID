using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.WechatWebLogin;

/// <summary>
/// 
/// </summary>
[Table("WechatUserIdentifier")]
public class WechatUserIdentifier
{
    /// <summary>
    /// 
    /// </summary>
    protected WechatUserIdentifier()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="openId"></param>
    /// <param name="personId"></param>
    public WechatUserIdentifier(string appId, string openId, string personId)
    {
        if (string.IsNullOrEmpty(appId))
        {
            throw new ArgumentException($"“{nameof(appId)}”不能是 Null 或为空", nameof(appId));
        }

        if (string.IsNullOrEmpty(openId))
        {
            throw new ArgumentException($"“{nameof(openId)}”不能是 Null 或为空", nameof(openId));
        }

        AppId = appId;
        OpenId = openId;
        PersonId = personId;
    }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string AppId { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string OpenId { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string PersonId { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime WhenCreated { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(200)]
    public string UserPrincipalName { get; protected internal set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string UserSecret { get; protected internal set; } = default!;
}
