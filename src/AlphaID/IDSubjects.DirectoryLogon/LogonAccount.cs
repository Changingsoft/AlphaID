using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.DirectoryServices;

namespace IDSubjects.DirectoryLogon;

/// <summary>
/// Logon Account
/// </summary>
[Table("LogonAccount")]
public class LogonAccount
{
    /// <summary>
    /// 
    /// </summary>
    protected internal LogonAccount() { }


    /// <summary>
    /// LogonId.
    /// </summary>
    [Key]
    [MaxLength(128), Unicode(false)]
    public string LogonId { get; protected internal set; } = default!;

    /// <summary>
    /// PersonId.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string PersonId { get; protected internal set; } = default!;


    /// <summary>
    /// 目录服务Id.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// 目录服务。
    /// </summary>
    [ForeignKey(nameof(ServiceId))]
    public virtual DirectoryService DirectoryService { get; protected set; } = default!;

    /// <summary>
    /// Gets directory entry.
    /// </summary>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public DirectoryEntry? GetDirectoryEntry()
    {
        Guid directoryObjectGUID;
        try
        {
            directoryObjectGUID = new(Convert.FromBase64String(this.LogonId));
        }
        catch (Exception)
        {

            throw;
        }

        using var root = new DirectoryEntry($"LDAP://{this.DirectoryService.ServerAddress}/{this.DirectoryService.RootDN}");
        using var searcher = new DirectorySearcher(root);
        searcher.Filter = $"(objectGUID={directoryObjectGUID.ToHexString()})";
        var result = searcher.FindOne();
        return result?.GetDirectoryEntry();
    }

}
