using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Validation;
using IDSubjects;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

public class PersonResourceOwnerPasswordValidator : ResourceOwnerPasswordValidator<NaturalPerson>
{
    private readonly NaturalPersonManager userManager;
    private readonly ILogger<ResourceOwnerPasswordValidator<NaturalPerson>> logger;

    public PersonResourceOwnerPasswordValidator(NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager, ILogger<ResourceOwnerPasswordValidator<NaturalPerson>> logger) : base(userManager, signInManager, logger)
    {
        this.userManager = userManager;
        this.logger = logger;
    }

    public override async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await this.userManager.FindByNameAsync(context.UserName);
        if (user != null)
        {
            if (!user.PasswordLastSet.HasValue || user.PasswordLastSet.Value < DateTime.Now.AddDays(-365.0))
            {
                this.logger.LogInformation("Authentication failed for username: {username}, reason: User must change password before first login.", context.UserName);
                context.Result = new GrantValidationResult(Duende.IdentityServer.Models.TokenRequestErrors.InvalidGrant);
                return;
            }
        }
        await base.ValidateAsync(context);
    }
}
