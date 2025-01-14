#nullable disable

using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace IdSubjects.DirectoryLogon;

public static class NaturalPersonExtensions
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public static void Apply(this NaturalPerson person, UserPrincipal userPrincipal)
    {
        userPrincipal.EmailAddress = person.Email;
        userPrincipal.GivenName = person.PersonName.GivenName;
        userPrincipal.Surname = person.PersonName.Surname;
        userPrincipal.DisplayName = person.PersonName.FullName;
        //userPrincipal.SamAccountName = person.UserName;

        var entry = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
        entry.Properties["mobile"].Value = person.PhoneNumber;
        entry.Properties["sAMAccountName"].Value = person.UserName;
        entry.CommitChanges();
    }
}