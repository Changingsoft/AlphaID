using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace IdSubjects.DirectoryLogon;
internal class UserPasswordInterceptor(DirectoryAccountManager accountManager) : IUserPasswordInterceptor
{
    private IEnumerable<DirectoryAccount> _accounts = [];
    private string? _password;

    public Task<IdentityResult> PasswordChangingAsync(NaturalPerson person, string? plainPassword, CancellationToken cancellation)
    {
        _accounts = accountManager.GetLogonAccounts(person);
        _password = plainPassword;
        return Task.FromResult(IdentityResult.Success);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    public Task PasswordChangedAsync(NaturalPerson person, CancellationToken cancellation)
    {
        foreach (var account in _accounts)
        {
            account.SetPassword(_password, !person.PasswordLastSet.HasValue);
        }
        return Task.CompletedTask;
    }
}
