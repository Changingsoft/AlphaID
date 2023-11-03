using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.LinearReferencing;
using System.ComponentModel;
using System.Security.Claims;

namespace AlphaID.EntityFramework;

public class NaturalPersonStore : IUserLoginStore<NaturalPerson>,
    IUserClaimStore<NaturalPerson>,
    IUserPasswordStore<NaturalPerson>,
    IUserSecurityStampStore<NaturalPerson>,
    IUserEmailStore<NaturalPerson>,
    IUserLockoutStore<NaturalPerson>,
    IUserPhoneNumberStore<NaturalPerson>,
    IQueryableUserStore<NaturalPerson>,
    IUserTwoFactorStore<NaturalPerson>,
    IUserAuthenticationTokenStore<NaturalPerson>,
    IUserAuthenticatorKeyStore<NaturalPerson>,
    IUserTwoFactorRecoveryCodeStore<NaturalPerson>
{

    IDSubjectsDbContext context;
    IdentityErrorDescriber? describer;

    public NaturalPersonStore(IDSubjectsDbContext context, IdentityErrorDescriber? describer = null)
    {
        this.context = context;
        this.describer = describer;
    }

    public IQueryable<NaturalPerson> Users => this.context.People;

    public Task AddClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            this.context.PersonClaims.Add(new NaturalPersonClaim()
            {
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            });
        }
        return Task.FromResult(false);
    }

    public Task AddLoginAsync(NaturalPerson user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        this.context.PersonLogins.Add(new NaturalPersonLogin()
        {
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName,
            ProviderKey = login.ProviderKey,
            UserId = user.Id,
        });
        return Task.FromResult(false);
    }

    public async Task<int> CountCodesAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        var mergedCodes = await this.GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken).ConfigureAwait(false) ?? "";
        if (mergedCodes.Length > 0)
            return mergedCodes.Split(';').Length;
        return 0;
    }

    public async Task<IdentityResult> CreateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        this.context.People.Add(user);
        await this.context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        this.context.People.Remove(user);
        await this.context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public void Dispose()
    {

    }

    public Task<NaturalPerson?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return this.context.People.SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public Task<NaturalPerson?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return this.context.People.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }

    public async Task<NaturalPerson?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var login = await this.context.PersonLogins.FirstOrDefaultAsync(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey, cancellationToken);
        return login?.User;
    }

    public Task<NaturalPerson?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return this.context.People.SingleOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken: cancellationToken);
    }

    public Task<int> GetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task<string?> GetAuthenticatorKeyAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return this.GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
    }

    public async Task<IList<Claim>> GetClaimsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return await this.context.PersonClaims.Where(c => c.UserId == user.Id).Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<string?> GetEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task<bool> GetLockoutEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LockoutEnabled);
    }

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LockoutEnd);
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return await this.context.PersonLogins
            .Where(l => l.UserId == user.Id)
            .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName))
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<string?> GetNormalizedEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedEmail);
    }

    public Task<string?> GetNormalizedUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task<string?> GetPasswordHashAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<string?> GetPhoneNumberAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public Task<string?> GetSecurityStampAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.SecurityStamp);
    }

    public async Task<string?> GetTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        var token = await this.context.PersonTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.LoginProvider == loginProvider && t.Name == name, cancellationToken: cancellationToken);
        return token?.Value;
    }

    public Task<bool> GetTwoFactorEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task<string> GetUserIdAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public async Task<IList<NaturalPerson>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        var query = from userClaim in this.context.PersonClaims
                    join user in this.context.People on userClaim.UserId equals user.Id
                    where userClaim.ClaimValue == claim.Value && userClaim.ClaimType == claim.Type
                    select user;
        return await query.ToListAsync();
    }

    public Task<bool> HasPasswordAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public Task<int> IncrementAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    public async Task<bool> RedeemCodeAsync(NaturalPerson user, string code, CancellationToken cancellationToken)
    {
        var mergedCodes = await this.GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken).ConfigureAwait(false) ?? "";
        var splitCodes = mergedCodes.Split(';');
        if (splitCodes.Contains(code))
        {
            var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
            await this.ReplaceCodesAsync(user, updatedCodes, cancellationToken).ConfigureAwait(false);
            return true;
        }
        return false;
    }

    public async Task RemoveClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            var matchedClaims = await this.context.PersonClaims.Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToListAsync(cancellationToken);
            foreach (var c in matchedClaims)
            {
                this.context.PersonClaims.Remove(c);
            }
        }
    }

    public async Task RemoveLoginAsync(NaturalPerson user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var login = await this.context.PersonLogins.FirstOrDefaultAsync(l => l.UserId == user.Id && l.LoginProvider == loginProvider && l.ProviderKey == providerKey, cancellationToken: cancellationToken);
        if (login != null)
        {
            this.context.PersonLogins.Remove(login);
        }
    }

    public async Task RemoveTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        var token = await this.context.PersonTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.LoginProvider == loginProvider && t.Name == name, cancellationToken: cancellationToken);
        if (token != null)
            this.context.PersonTokens.Remove(token);
    }

    public async Task ReplaceClaimAsync(NaturalPerson user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        var matchedClaims = await this.context.PersonClaims.Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToListAsync(cancellationToken);
        foreach (var matchedClaim in matchedClaims)
        {
            matchedClaim.ClaimValue = newClaim.Value;
            matchedClaim.ClaimType = newClaim.Type;
        }
    }

    public async Task ReplaceCodesAsync(NaturalPerson user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        var mergedCodes = string.Join(";", recoveryCodes);
        await this.SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
    }

    public Task ResetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public async Task SetAuthenticatorKeyAsync(NaturalPerson user, string key, CancellationToken cancellationToken)
    {
        await this.SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
    }

    public Task SetEmailAsync(NaturalPerson user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(NaturalPerson user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetLockoutEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetLockoutEndDateAsync(NaturalPerson user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(NaturalPerson user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(NaturalPerson user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(NaturalPerson user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task SetPhoneNumberAsync(NaturalPerson user, string? phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
        return Task.CompletedTask;
    }

    public Task SetPhoneNumberConfirmedAsync(NaturalPerson user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetSecurityStampAsync(NaturalPerson user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public async Task SetTokenAsync(NaturalPerson user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
    {
        var token = await this.context.PersonTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.LoginProvider == loginProvider && t.Name == name, cancellationToken: cancellationToken);
        if (token == null)
        {
            this.context.PersonTokens.Add(new NaturalPersonToken()
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                User = user,
            });
        }
        else
        {
            token.Value = value;
        }
    }

    public Task SetTwoFactorEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(NaturalPerson user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        this.context.People.Update(user);
        await this.context.SaveChangesAsync();
        return IdentityResult.Success;
    }

    private const string InternalLoginProvider = "[AspNetUserStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private const string RecoveryCodeTokenName = "RecoveryCodes";
}
