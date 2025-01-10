using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.DirectoryLogon;

/// <summary>
///     Logon Account
/// </summary>
[Table("LogonAccount")]
[PrimaryKey(nameof(PersonId), nameof(ServiceId))]
public class DirectoryAccount
{
    /// <summary>
    /// </summary>
    protected DirectoryAccount()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="serviceDescriptor"></param>
    /// <param name="personId"></param>
    public DirectoryAccount(DirectoryServiceDescriptor serviceDescriptor, string personId)
    {
        DirectoryServiceDescriptor = serviceDescriptor;
        ServiceId = serviceDescriptor.Id;
        PersonId = personId;
    }

    /// <summary>
    ///     PersonId.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string PersonId { get; protected internal set; } = null!;

    /// <summary>
    ///     目录对象的objectGUID。
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string ObjectId { get; protected internal set; } = null!;

    /// <summary>
    ///     目录服务Id.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    ///     目录服务。
    /// </summary>
    [ForeignKey(nameof(ServiceId))]
    public DirectoryServiceDescriptor DirectoryServiceDescriptor { get; protected set; } = null!;

    [SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    internal UserPrincipal? GetUserPrincipal()
    {
        PrincipalContext context = DirectoryServiceDescriptor.GetRootContext();
        return UserPrincipal.FindByIdentity(context, ObjectId);
    }

    [SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    internal void SetPassword(string? password, bool mustChangePassword = false)
    {
        using UserPrincipal? user = GetUserPrincipal();
        ArgumentNullException.ThrowIfNull(user);

        user.SetPassword(password);

        var entry = (DirectoryEntry)user.GetUnderlyingObject();
        if (mustChangePassword)
            entry.Properties["pwdLastSet"][0] = 0;
    }
}