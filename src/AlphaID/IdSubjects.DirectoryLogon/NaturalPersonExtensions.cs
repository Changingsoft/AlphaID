using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
#nullable disable

namespace IdSubjects.DirectoryLogon;
internal static class NaturalPersonExtensions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public static void Apply(this NaturalPerson person, UserPrincipal userPrincipal)
    {
        userPrincipal.EmailAddress = person.Email;
        userPrincipal.GivenName = person.PersonName.GivenName;
        userPrincipal.Surname = person.PersonName.Surname;
        userPrincipal.DisplayName = person.PersonName.FullName;
        //userPrincipal.SamAccountName = person.UserName;

        DirectoryEntry entry = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
        entry.Properties["mobile"].Value = person.PhoneNumber;
        entry.Properties["sAMAccountName"].Value = person.UserName;
        entry.CommitChanges();
    }
}
