
namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Logon Account
/// </summary>
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
    public string UserId { get; protected internal set; } = null!;

    /// <summary>
    /// 目录对象的objectGUID。
    /// </summary>
    public string ObjectId { get; protected internal set; } = null!;

    /// <summary>
    /// 目录服务Id.
    /// </summary>
    public int ServiceId { get; protected set; }

    /// <summary>
    /// 目录服务。
    /// </summary>
    public virtual DirectoryService DirectoryService { get; protected set; } = null!;
}