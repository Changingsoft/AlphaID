using IdSubjects.Diagnostics;
using Microsoft.Extensions.Logging;

namespace IdSubjects.DirectoryLogon;
internal class DirectoryAccountUpdateInterceptor(ILogger<DirectoryAccountUpdateInterceptor>? logger, IDirectoryAccountStore store) : NaturalPersonUpdateInterceptor
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    public override Task PostUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        var accounts = store.Accounts.Where(a => a.PersonId == person.Id);
        foreach (var account in accounts)
        {
            var svc = account.DirectoryServiceDescriptor;
            using var context = svc.GetRootContext();

            var userPrincipal = account.GetUserPrincipal();
            if (userPrincipal == null)
            {
                return Task.CompletedTask;
            }

            person.Apply(userPrincipal);
            userPrincipal.Save();
        }
        return Task.CompletedTask;
    }
}
