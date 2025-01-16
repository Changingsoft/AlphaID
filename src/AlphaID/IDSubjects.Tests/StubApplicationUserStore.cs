using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Tests;

public class StubApplicationUserStore(IdentityErrorDescriber describer)
    : UserStoreBase<ApplicationUser, string, IdentityUserClaim<string>, IdentityUserLogin<string>,
        IdentityUserToken<string>>(describer), IApplicationUserStore
{
    private readonly HashSet<ApplicationUser> _set = [];
    private readonly HashSet<ApplicationUser> _trackedSet = [];

    public override IQueryable<ApplicationUser> Users => _set.AsQueryable();


    public override Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        _trackedSet.Add(user);
        _set.Add(Clone(user)!);
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        _trackedSet.Remove(user);
        ApplicationUser origin = _set.Single(p => p.Id == user.Id);
        _set.Remove(origin);
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        ApplicationUser? result = _trackedSet.FirstOrDefault(p => p.NormalizedEmail == normalizedEmail);
        result ??= Clone(_set.FirstOrDefault(p => p.NormalizedEmail == normalizedEmail));

        if (result != null)
            _trackedSet.Add(result);
        return Task.FromResult(result);
    }

    public Task<ApplicationUser?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        ApplicationUser? result = _trackedSet.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
        result ??= Clone(_set.FirstOrDefault(p => p.PhoneNumber == phoneNumber));

        if (result != null)
            _trackedSet.Add(result);
        return Task.FromResult(result);
    }

    public Task<ApplicationUser?> GetOriginalAsync(ApplicationUser person, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_set.FirstOrDefault(p => p.Id == person.Id));
    }

    public override Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ApplicationUser? result = _trackedSet.FirstOrDefault(p => p.Id == userId);
        if (result == null)
        {
            result = Clone(_set.FirstOrDefault(p => p.Id == userId));
            if (result != null)
                _trackedSet.Add(result);
        }

        return Task.FromResult(result);
    }

    public override Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        ApplicationUser? result = _trackedSet.FirstOrDefault(p => p.NormalizedUserName == normalizedUserName);
        result ??= Clone(_set.FirstOrDefault(p => p.NormalizedUserName == normalizedUserName));

        if (result != null)
            _trackedSet.Add(result);
        return Task.FromResult(result);
    }

    protected override Task<ApplicationUser?> FindUserAsync(string userId, CancellationToken cancellationToken)
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

    public override Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task AddClaimsAsync(ApplicationUser user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task ReplaceClaimAsync(ApplicationUser user,
        Claim claim,
        Claim newClaim,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task RemoveClaimsAsync(ApplicationUser user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    protected override Task<IdentityUserToken<string>?> FindTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task AddUserTokenAsync(IdentityUserToken<string> token)
    {
        throw new NotImplementedException();
    }

    protected override Task RemoveUserTokenAsync(IdentityUserToken<string> token)
    {
        throw new NotImplementedException();
    }

    public override Task AddLoginAsync(ApplicationUser user,
        UserLoginInfo login,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task RemoveLoginAsync(ApplicationUser user,
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }


    public override Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        _trackedSet.Add(user);

        ApplicationUser? found = _set.FirstOrDefault(p => p.Id == user.Id);
        if (found != null)
        {
            _set.Remove(found);
            _set.Add(Clone(user)!);
        }

        return Task.FromResult(IdentityResult.Success);
    }

    private static T? Clone<T>(T? source)
    {
        if (source == null)
            return source;
        return TransExp<T, T>.Trans(source);
    }
}