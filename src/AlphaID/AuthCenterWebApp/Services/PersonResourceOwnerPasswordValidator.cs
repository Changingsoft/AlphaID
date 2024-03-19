using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Validation;
using IdSubjects;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

public class PersonResourceOwnerPasswordValidator(NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager, ILogger<ResourceOwnerPasswordValidator<NaturalPerson>> logger) : ResourceOwnerPasswordValidator<NaturalPerson>(userManager, signInManager, logger)
{
    public override async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await userManager.FindByNameAsync(context.UserName);
        if (user != null)
        {
            if (!user.PasswordLastSet.HasValue || user.PasswordLastSet.Value < DateTime.UtcNow.AddDays(-365.0))
            {
                logger.LogInformation("Authentication failed for username: {username}, reason: User must change password before first login.", context.UserName);
                context.Result = new GrantValidationResult(Duende.IdentityServer.Models.TokenRequestErrors.InvalidGrant);
                return;
            }
        }
        await base.ValidateAsync(context);
    }
}
