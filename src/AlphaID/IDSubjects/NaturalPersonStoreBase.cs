using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
/// </summary>
public abstract class NaturalPersonStoreBase : INaturalPersonStore
{
    private const string InternalLoginProvider = "[AspNetUserStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private const string RecoveryCodeTokenName = "RecoveryCodes";

    /// <summary>
    ///     获取NaturalPerson的可查询集合。
    /// </summary>
    public abstract IQueryable<NaturalPerson> Users { get; }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="claims"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task AddClaimsAsync(NaturalPerson user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="login"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task AddLoginAsync(NaturalPerson user, UserLoginInfo login, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<int> CountCodesAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        string mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken)
            .ConfigureAwait(false) ?? "";
        return mergedCodes.Length > 0 ? mergedCodes.Split(';').Length : 0;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IdentityResult> CreateAsync(NaturalPerson user, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IdentityResult> DeleteAsync(NaturalPerson user, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="normalizedEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<NaturalPerson?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<NaturalPerson?>
        FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="person"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<NaturalPerson?> GetOriginalAsync(NaturalPerson person, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<NaturalPerson?> FindByIdAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="loginProvider"></param>
    /// <param name="providerKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<NaturalPerson?> FindByLoginAsync(string loginProvider,
        string providerKey,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="normalizedUserName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<NaturalPerson?>
        FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<int> GetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetAuthenticatorKeyAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IList<Claim>> GetClaimsAsync(NaturalPerson user, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<bool> GetEmailConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<bool> GetLockoutEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LockoutEnabled);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LockoutEnd);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IList<UserLoginInfo>> GetLoginsAsync(NaturalPerson user, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetNormalizedEmailAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedEmail);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetNormalizedUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.NormalizedUserName);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetPasswordHashAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetPhoneNumberAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumber);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<bool> GetPhoneNumberConfirmedAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetSecurityStampAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.SecurityStamp);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<string?> GetTokenAsync(NaturalPerson user,
        string loginProvider,
        string name,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<bool> GetTwoFactorEnabledAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.TwoFactorEnabled);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string> GetUserIdAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetUserNameAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.UserName);
    }

    /// <summary>
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IList<NaturalPerson>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<bool> HasPasswordAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<int> IncrementAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<bool> RedeemCodeAsync(NaturalPerson user,
        string code,
        CancellationToken cancellationToken)
    {
        string mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken)
            .ConfigureAwait(false) ?? "";
        string[] splitCodes = mergedCodes.Split(';');
        if (splitCodes.Contains(code))
        {
            var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
            await ReplaceCodesAsync(user, updatedCodes, cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="claims"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task RemoveClaimsAsync(NaturalPerson user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="loginProvider"></param>
    /// <param name="providerKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task RemoveLoginAsync(NaturalPerson user,
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task RemoveTokenAsync(NaturalPerson user,
        string loginProvider,
        string name,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="claim"></param>
    /// <param name="newClaim"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task ReplaceClaimAsync(NaturalPerson user,
        Claim claim,
        Claim newClaim,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="recoveryCodes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task ReplaceCodesAsync(NaturalPerson user,
        IEnumerable<string> recoveryCodes,
        CancellationToken cancellationToken)
    {
        string mergedCodes = string.Join(";", recoveryCodes);
        return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task ResetAccessFailedCountAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetAuthenticatorKeyAsync(NaturalPerson user, string key, CancellationToken cancellationToken)
    {
        return SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetEmailAsync(NaturalPerson user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="confirmed"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetEmailConfirmedAsync(NaturalPerson user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="enabled"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetLockoutEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="lockoutEnd"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetLockoutEndDateAsync(NaturalPerson user,
        DateTimeOffset? lockoutEnd,
        CancellationToken cancellationToken)
    {
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="normalizedEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetNormalizedEmailAsync(NaturalPerson user,
        string? normalizedEmail,
        CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="normalizedName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetNormalizedUserNameAsync(NaturalPerson user,
        string? normalizedName,
        CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName ?? throw new ArgumentNullException(nameof(normalizedName));
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="passwordHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetPasswordHashAsync(NaturalPerson user,
        string? passwordHash,
        CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetPhoneNumberAsync(NaturalPerson user,
        string? phoneNumber,
        CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="confirmed"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetPhoneNumberConfirmedAsync(NaturalPerson user,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="stamp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetSecurityStampAsync(NaturalPerson user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     为用户设置Token。
    /// </summary>
    /// <param name="user"></param>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task SetTokenAsync(NaturalPerson user,
        string loginProvider,
        string name,
        string? value,
        CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="enabled"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetTwoFactorEnabledAsync(NaturalPerson user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetUserNameAsync(NaturalPerson user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IdentityResult> UpdateAsync(NaturalPerson user, CancellationToken cancellationToken);
}