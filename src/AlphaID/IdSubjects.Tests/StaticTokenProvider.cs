using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Tests;

internal sealed class StaticTokenProvider : IUserTwoFactorTokenProvider<ApplicationUser>
{
    public async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return MakeToken(purpose, await manager.GetUserIdAsync(user));
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return token == MakeToken(purpose, await manager.GetUserIdAsync(user));
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return Task.FromResult(true);
    }

    private static string MakeToken(string purpose, string userId)
    {
        return string.Join(":", userId, purpose, "ImmaToken");
    }
}
