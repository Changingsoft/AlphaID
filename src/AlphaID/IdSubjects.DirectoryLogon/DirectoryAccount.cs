using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Logon Account
/// </summary>
[Table("LogonAccount")]
[PrimaryKey(nameof(ObjectId), nameof(ServiceId))]
public class DirectoryAccount
{
    /// <summary>
    /// </summary>
    protected DirectoryAccount()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    /// <param name="userId"></param>
    /// <param name="objectId"></param>
    public DirectoryAccount(DirectoryService service, string userId, string objectId)
    {
        DirectoryService = service;
        ServiceId = service.Id;
        UserId = userId;
        ObjectId = objectId;
    }

    /// <summary>
    /// UserId.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string UserId { get; protected internal set; } = null!;

    /// <summary>
    /// 目录对象的objectGUID。
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string ObjectId { get; protected internal set; } = null!;

    /// <summary>
    /// 目录服务Id.
    /// </summary>
    public int ServiceId { get; protected set; }

    /// <summary>
    /// 目录服务。
    /// </summary>
    [ForeignKey(nameof(ServiceId))]
    public virtual DirectoryService DirectoryService { get; protected set; } = null!;
}