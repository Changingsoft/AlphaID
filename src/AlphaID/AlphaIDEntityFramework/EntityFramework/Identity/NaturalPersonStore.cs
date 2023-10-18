using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AlphaIDEntityFramework.EntityFramework.Identity;

public class NaturalPersonStore : INaturalPersonStore //, INaturalPersonChineseIDCardStore
{
    private readonly IDSubjectsDbContext context;

    public NaturalPersonStore(IDSubjectsDbContext context)
    {
        this.context = context;
    }

    public IQueryable<NaturalPerson> Users => this.context.People;

    public async Task AddClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            var personClaim = new NaturalPersonClaim(user, claim.Type, claim.Value);
            this.context.NaturalPersonClaims.Add(personClaim);
        }
        _ = await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddLoginAsync(NaturalPerson user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        var personLogin = new NaturalPersonLogin(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName, user);
        this.context.NaturalPersonLogins.Add(personLogin);
        _ = await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountCodesAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        var mergedCodes = await this.GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
        return mergedCodes.Length > 0 ? mergedCodes.Split(';').Length : 0;
    }

    public async Task<IdentityResult> CreateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        this.context.People.Add(user);
        _ = await this.context.SaveChangesAsync(cancellationToken);
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
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task<NaturalPerson?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return this.context.People.SingleOrDefaultAsync(p => p.Email == normalizedEmail, cancellationToken);
    }

    public async Task<NaturalPerson?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await this.context.People.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
    }

    public async Task<NaturalPerson?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var result = await this.context.NaturalPersonLogins.SingleOrDefaultAsync(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey, cancellationToken: cancellationToken);
        return result?.Person;
    }

    public Task<NaturalPerson?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return this.context.People.SingleOrDefaultAsync(p => p.UserName == normalizedUserName, cancellationToken: cancellationToken);
    }

    public Task<int> GetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task<string?> GetAuthenticatorKeyAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return this.GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
    }

    public async Task<IList<Claim>> GetClaimsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return await this.context.NaturalPersonClaims.Where(p => p.UserId == user.Id).Select(p => p.ToClaim()).ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<string?> GetEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task<bool> GetLockoutEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.LockoutEnabled);
    }

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.LockoutEnd);
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return await (from login in this.context.NaturalPersonLogins
                      where login.UserId == user.Id
                      select new UserLoginInfo(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName)).ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<string?> GetNormalizedEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Email?.ToUpper());
    }

    public Task<string?> GetNormalizedUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.UserName?.ToUpper());
    }

    public Task<string?> GetPasswordHashAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.PasswordHash);
    }

    public Task<string?> GetPhoneNumberAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public Task<string?> GetSecurityStampAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(user.SecurityStamp);
    }

    public async Task<string?> GetTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        var entry = await this.context.NaturalPersonTokens.SingleOrDefaultAsync(p => p.UserId == user.Id && p.LoginProvider == loginProvider && p.Name == name, cancellationToken: cancellationToken);
        return entry?.Value;
    }

    public Task<bool> GetTwoFactorEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task<string> GetUserIdAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string?> GetUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(user.UserName);
    }

    public async Task<IList<NaturalPerson>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var query = from userClaims in this.context.NaturalPersonClaims
                    join person in this.context.People on userClaims.UserId equals person.Id
                    where userClaims.ClaimValue == claim.Value && userClaims.ClaimType == claim.Type
                    select person;
        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<bool> HasPasswordAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public Task<int> IncrementAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    public async Task<bool> RedeemCodeAsync(NaturalPerson user, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var mergedCodes = await this.GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
        var splitCodes = mergedCodes.Split(';');
        if (splitCodes.Contains(code))
        {
            var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
            await this.ReplaceCodesAsync(user, updatedCodes, cancellationToken);
            return true;
        }
        return false;

    }

    public async Task RemoveClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var claim in claims)
        {
            var matchedClaim = this.context.NaturalPersonClaims.FirstOrDefault(p => p.UserId == user.Id && p.ClaimType == claim.Type && p.ClaimValue == claim.Value);
            if (matchedClaim != null)
            {
                this.context.NaturalPersonClaims.Remove(matchedClaim);
            }
        }
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveLoginAsync(NaturalPerson user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var login = this.context.NaturalPersonLogins.FirstOrDefault(p => p.UserId == user.Id && p.LoginProvider == loginProvider && p.ProviderKey == providerKey);
        if (login != null)
        {
            this.context.NaturalPersonLogins.Remove(login);
        }
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        var token = this.context.NaturalPersonTokens.FirstOrDefault(p => p.UserId == user.Id && p.LoginProvider == loginProvider && p.Name == name);
        if (token != null)
        {
            this.context.NaturalPersonTokens.Remove(token);
        }
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceClaimAsync(NaturalPerson user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        var claims = this.context.NaturalPersonClaims.Where(p => p.UserId == user.Id && p.ClaimValue == claim.Value && p.ClaimType == claim.Type).ToList();
        foreach (var c in claims)
        {
            c.ClaimType = newClaim.Type;
            c.ClaimValue = newClaim.Value;
            this.context.NaturalPersonClaims.Update(c);
        }
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public Task ReplaceCodesAsync(NaturalPerson user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        var mergedCodes = string.Join(";", recoveryCodes);
        return this.SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
    }

    public Task ResetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public Task SetAuthenticatorKeyAsync(NaturalPerson user, string key, CancellationToken cancellationToken)
    {
        return this.SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
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
        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(NaturalPerson user, string? normalizedName, CancellationToken cancellationToken)
    {
        //不再使用NormalizedName字段，此处不做任何事。
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

    public Task SetTwoFactorEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(NaturalPerson user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName ?? "";
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        this.context.Entry(user).State = EntityState.Modified;
        _ = await this.context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    protected ValueTask<NaturalPersonToken?> FindTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        return this.context.NaturalPersonTokens.FindAsync(new object?[] { user.Id, loginProvider, name }, cancellationToken: cancellationToken);
    }

    protected Task AddUserTokenAsync(NaturalPersonToken token)
    {
        this.context.Add(token);
        return Task.CompletedTask;
    }


    private const string InternalLoginProvider = "[AspNetUserStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private const string RecoveryCodeTokenName = "RecoveryCodes";
}
