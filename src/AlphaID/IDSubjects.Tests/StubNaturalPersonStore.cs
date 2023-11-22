using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdSubjects.Tests;

public class StubNaturalPersonStore : NaturalPersonStoreBase
{
    private readonly HashSet<NaturalPerson> set = new();

    public override IQueryable<NaturalPerson> Users => this.set.AsQueryable();

    public override Task AddClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task AddLoginAsync(NaturalPerson user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IdentityResult> CreateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
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
        return Task.FromResult(this.set.FirstOrDefault(p => p.NormalizedEmail == normalizedEmail));
    }

    public override Task<NaturalPerson?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.set.FirstOrDefault(p => p.PhoneNumber == phoneNumber));
    }

    public override Task<NaturalPerson?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.set.FirstOrDefault(p => p.Id == userId));
    }

    public override Task<NaturalPerson?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<NaturalPerson?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.set.FirstOrDefault(p => p.NormalizedUserName == normalizedUserName));
    }

    public override Task<IList<Claim>> GetClaimsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IList<UserLoginInfo>> GetLoginsAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<string?> GetTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IList<NaturalPerson>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveClaimsAsync(NaturalPerson user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveLoginAsync(NaturalPerson user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveTokenAsync(NaturalPerson user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task ReplaceClaimAsync(NaturalPerson user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task SetTokenAsync(NaturalPerson user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IdentityResult> UpdateAsync(NaturalPerson user, CancellationToken cancellationToken)
    {
        //do nothing
        return Task.FromResult(IdentityResult.Success);
    }
}
