using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace IdSubjects.DirectoryLogon;
internal class UserPasswordInterceptor(DirectoryAccountManager accountManager) : IUserPasswordInterceptor
{
    private IEnumerable<DirectoryAccount> accounts = [];
    private string? password;

    public Task<IdentityResult> PasswordChangingAsync(NaturalPerson person, string? plainPassword, CancellationToken cancellation)
    {
        this.accounts = accountManager.GetLogonAccounts(person);
        this.password = plainPassword;
        return Task.FromResult(IdentityResult.Success);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    public Task PasswordChangedAsync(NaturalPerson person, CancellationToken cancellation)
    {
        foreach (var account in this.accounts)
        {
            account.SetPassword(this.password, !person.PasswordLastSet.HasValue);
        }
        return Task.CompletedTask;
    }
}
