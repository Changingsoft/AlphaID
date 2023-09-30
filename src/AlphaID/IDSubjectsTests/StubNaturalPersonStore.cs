using IDSubjects;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IDSubjectsTests;

internal class StubNaturalPersonStore : INaturalPersonStore
{
    private readonly HashSet<NaturalPerson> set;
    private readonly HashSet<NaturalPersonClaim> claims;
    private readonly HashSet<NaturalPersonLogin> logins;
    private readonly HashSet<NaturalPersonToken> tokens;

    public StubNaturalPersonStore()
    {
        this.set = new HashSet<NaturalPerson>();
        this.claims = new HashSet<NaturalPersonClaim>();
        this.logins = new HashSet<NaturalPersonLogin>();
        this.tokens = new HashSet<NaturalPersonToken>();
    }

    public IQueryable<NaturalPerson> Users => this.set.AsQueryable();

    public Task AddClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            this.claims.Add(new NaturalPersonClaim(user, claim.Type, claim.Value));
        }
        return Task.CompletedTask;
    }

    public Task AddLoginAsync(NaturalPerson user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        this.logins.Add(new NaturalPersonLogin(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName, user));
        return Task.CompletedTask;
    }

    public Task AddToRoleAsync(NaturalPerson user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountCodesAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> CreateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.Id = Guid.NewGuid().ToString().ToLower();
        this.set.Add(user);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        this.set.Remove(user);
        return Task.FromResult(IdentityResult.Success);
    }

    public void Dispose()
    {
        //
    }

    public Task<NaturalPerson?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var person = this.set.FirstOrDefault(p => p.Email!.Equals(normalizedEmail, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(person);
    }

    public Task<NaturalPerson?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var person = this.set.FirstOrDefault(p => p.Id == userId);
        return Task.FromResult(person);
    }

    public Task<NaturalPerson?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var login = this.logins.FirstOrDefault(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey);
        if (login == null)
            return Task.FromResult(default(NaturalPerson));
        var person = this.set.FirstOrDefault(p => p.Id == login.UserId);
        return Task.FromResult(person);
    }

    public Task<NaturalPerson?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var person = this.set.FirstOrDefault(p => p.UserName.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(person);
    }

    public Task<int> GetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetAuthenticatorKeyAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Claim>> GetClaimsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GetLockoutEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetPasswordHashAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetPhoneNumberAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> GetRolesAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetSecurityStampAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(user.SecurityStamp);
    }

    public Task<string?> GetTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var token = this.tokens.FirstOrDefault(p => p.LoginProvider == loginProvider && p.Name == name);
        return Task.FromResult(token?.Value);
    }

    public Task<bool> GetTwoFactorEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task<string> GetUserIdAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(user.UserName);
    }

    public Task<IList<NaturalPerson>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<NaturalPerson>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasPasswordAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> IncrementAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(NaturalPerson user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RedeemCodeAsync(NaturalPerson user, string code, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(NaturalPerson user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveLoginAsync(NaturalPerson user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var login = this.logins.FirstOrDefault(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey);
        if (login != null)
            this.logins.Remove(login);
        return Task.CompletedTask;
    }

    public Task RemoveTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ReplaceClaimAsync(NaturalPerson user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ReplaceCodesAsync(NaturalPerson user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ResetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetAuthenticatorKeyAsync(NaturalPerson user, string key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetEmailAsync(NaturalPerson user, string? email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(NaturalPerson user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetLockoutEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetLockoutEndDateAsync(NaturalPerson user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedEmailAsync(NaturalPerson user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        //do nothing.
        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(NaturalPerson user, string? normalizedName, CancellationToken cancellationToken)
    {
        //do nothing.
        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(NaturalPerson user, string? passwordHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetPhoneNumberAsync(NaturalPerson user, string? phoneNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetPhoneNumberConfirmedAsync(NaturalPerson user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetSecurityStampAsync(NaturalPerson user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public async Task SetTokenAsync(NaturalPerson user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var token = await this.FindTokenAsync(user, loginProvider, name, cancellationToken);
        if (token == null)
        {
            await this.AddUserTokenAsync(new NaturalPersonToken(user, loginProvider, name, value));
        }
        else
        {
            token.Value = value;
        }
    }

    protected ValueTask<NaturalPersonToken?> FindTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(this.tokens.FirstOrDefault(p => p.LoginProvider == loginProvider && p.Name == name && p.UserId == user.Id));
    }

    protected Task AddUserTokenAsync(NaturalPersonToken token)
    {
        this.tokens.Add(token);
        return Task.CompletedTask;
    }

    public Task SetTwoFactorEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetUserNameAsync(NaturalPerson user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        //do nothing.
        return Task.FromResult(IdentityResult.Success);
    }
}
