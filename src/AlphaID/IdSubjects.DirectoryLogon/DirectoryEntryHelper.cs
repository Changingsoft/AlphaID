using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;

namespace IdSubjects.DirectoryLogon;

internal class DirectoryEntryHelper
{
    /// <summary>
    /// Gets a directory entry instance.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public static DirectoryEntry GetRootEntry(DirectoryService directoryService)
    {
        var host = new Uri($"LDAP://{directoryService.ServerAddress}");
        var fqdn = new Uri(host, directoryService.RootDn);
        AuthenticationTypes authenticationFlag =
            AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
        DirectoryEntry entry = new($"LDAP://{fqdn.Authority}{fqdn.PathAndQuery}", null, null, authenticationFlag);
        if (!string.IsNullOrEmpty(directoryService.UserName) && !string.IsNullOrEmpty(directoryService.Password))
        {
            entry.Username = directoryService.UserName;
            entry.Password = directoryService.Password;
        }

        return entry;
    }
}
