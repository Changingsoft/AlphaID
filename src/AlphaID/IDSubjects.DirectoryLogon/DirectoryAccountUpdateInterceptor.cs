using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using IdSubjects.Diagnostics;

namespace IdSubjects.DirectoryLogon;

internal class DirectoryAccountUpdateInterceptor(IDirectoryAccountStore store) : NaturalPersonUpdateInterceptor
{
    [SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    public override Task PostUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        IQueryable<DirectoryAccount> accounts = store.Accounts.Where(a => a.PersonId == person.Id);
        foreach (DirectoryAccount account in accounts)
        {
            DirectoryServiceDescriptor svc = account.DirectoryServiceDescriptor;
            using PrincipalContext context = svc.GetRootContext();

            UserPrincipal? userPrincipal = account.GetUserPrincipal();
            if (userPrincipal == null) return Task.CompletedTask;

            person.Apply(userPrincipal);
            userPrincipal.Save();
        }

        return Task.CompletedTask;
    }
}