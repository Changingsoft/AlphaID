using IDSubjects;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IDSubjectsTests;

internal class StubNaturalPersonStore : UserStoreBase<NaturalPerson, string, IdentityUserClaim<string>, IdentityUserLogin<string>, IdentityUserToken<string>>
{
    private readonly HashSet<NaturalPerson> set;
    private readonly HashSet<IdentityUserClaim<string>> claims;
    private readonly HashSet<IdentityUserLogin<string>> logins;
    private readonly HashSet<IdentityUserToken<string>> tokens;

    public StubNaturalPersonStore() : base(new IdentityErrorDescriber())
    {
        this.set = new HashSet<NaturalPerson>();
        this.claims = new HashSet<IdentityUserClaim<string>>();
        this.logins = new HashSet<IdentityUserLogin<string>>();
        this.tokens = new HashSet<IdentityUserToken<string>>();
    }

    public override IQueryable<NaturalPerson> Users => this.set.AsQueryable();

    public override Task AddClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            this.claims.Add(new IdentityUserClaim<string>
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                UserId = user.Id,
            });
        }
        return Task.CompletedTask;
    }

    public override Task AddLoginAsync(NaturalPerson user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        this.logins.Add(new IdentityUserLogin<string>
        {
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName,
            ProviderKey = login.ProviderKey,
            UserId = user.Id,
        });
        return Task.CompletedTask;
    }


    public override Task<int> CountCodesAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IdentityResult> CreateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.Id = Guid.NewGuid().ToString().ToLower();
        this.set.Add(user);
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> DeleteAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        this.set.Remove(user);
        return Task.FromResult(IdentityResult.Success);
    }


    public override Task<NaturalPerson?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var person = this.set.FirstOrDefault(p => p.Email!.Equals(normalizedEmail, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(person);
    }

    public override Task<NaturalPerson?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var person = this.set.FirstOrDefault(p => p.Id == userId);
        return Task.FromResult(person);
    }

    public override Task<NaturalPerson?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var login = this.logins.FirstOrDefault(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey);
        if (login == null)
            return Task.FromResult(default(NaturalPerson));
        var person = this.set.FirstOrDefault(p => p.Id == login.UserId);
        return Task.FromResult(person);
    }

    public override Task<NaturalPerson?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var person = this.set.FirstOrDefault(p => p.NormalizedUserName == normalizedUserName);
        return Task.FromResult(person);
    }

    public override Task<int> GetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.AccessFailedCount);
    }

    public override Task<string?> GetAuthenticatorKeyAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IList<Claim>> GetClaimsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<string?> GetEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Email);
    }

    public override Task<bool> GetEmailConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> GetLockoutEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<DateTimeOffset?> GetLockoutEndDateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IList<UserLoginInfo>> GetLoginsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<string?> GetNormalizedEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<string?> GetNormalizedUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<string?> GetPasswordHashAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<string?> GetPhoneNumberAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> GetPhoneNumberConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> GetRolesAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<string?> GetSecurityStampAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(user.SecurityStamp);
    }

    public override Task<string?> GetTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var token = this.tokens.FirstOrDefault(p => p.LoginProvider == loginProvider && p.Name == name);
        return Task.FromResult(token?.Value);
    }

    public override Task<bool> GetTwoFactorEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public override Task<string> GetUserIdAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Id);
    }

    public override Task<string?> GetUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(user.UserName);
    }

    public override Task<IList<NaturalPerson>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<NaturalPerson>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> HasPasswordAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<int> IncrementAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(NaturalPerson user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> RedeemCodeAsync(NaturalPerson user, string code, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(NaturalPerson user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveLoginAsync(NaturalPerson user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var login = this.logins.FirstOrDefault(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey);
        if (login != null)
            this.logins.Remove(login);
        return Task.CompletedTask;
    }

    public override Task RemoveTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task ReplaceClaimAsync(NaturalPerson user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task ReplaceCodesAsync(NaturalPerson user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task ResetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetAuthenticatorKeyAsync(NaturalPerson user, string key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetEmailAsync(NaturalPerson user, string? email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.Email = email;
        return Task.CompletedTask;
    }

    public override Task SetEmailConfirmedAsync(NaturalPerson user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetLockoutEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    public override Task SetLockoutEndDateAsync(NaturalPerson user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetNormalizedEmailAsync(NaturalPerson user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        //do nothing.
        return Task.CompletedTask;
    }

    public override Task SetNormalizedUserNameAsync(NaturalPerson user, string? normalizedName, CancellationToken cancellationToken)
    {
        //do nothing.
        return Task.CompletedTask;
    }

    public override Task SetPasswordHashAsync(NaturalPerson user, string? passwordHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetPhoneNumberAsync(NaturalPerson user, string? phoneNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetPhoneNumberConfirmedAsync(NaturalPerson user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetSecurityStampAsync(NaturalPerson user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public override async Task SetTokenAsync(NaturalPerson user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var token = await this.FindTokenAsync(user, loginProvider, name, cancellationToken);
        if (token == null)
        {
            await this.AddUserTokenAsync(new IdentityUserToken<string>
            {
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                UserId = user.Id
            });
        }
        else
        {
            token.Value = value;
        }
    }

    protected override Task<IdentityUserToken<string>?> FindTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(this.tokens.FirstOrDefault(p => p.LoginProvider == loginProvider && p.Name == name && p.UserId == user.Id));
    }

    protected override Task AddUserTokenAsync(IdentityUserToken<string> token)
    {
        this.tokens.Add(token);
        return Task.CompletedTask;
    }

    public override Task SetTwoFactorEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetUserNameAsync(NaturalPerson user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IdentityResult> UpdateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        //do nothing.
        return Task.FromResult(IdentityResult.Success);
    }

    protected override Task<NaturalPerson?> FindUserAsync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task<IdentityUserLogin<string>?> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task<IdentityUserLogin<string>?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


    protected override Task RemoveUserTokenAsync(IdentityUserToken<string> token)
    {
        throw new NotImplementedException();
    }
}
