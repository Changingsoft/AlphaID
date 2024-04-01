using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Validation;
using IdSubjects;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

/// <summary>
/// 自然人资源所有者验证器。
/// </summary>
/// <remarks>
/// 该验证器会考虑<see cref="NaturalPerson.PasswordLastSet"></see>属性以确保密码有效。
/// </remarks>
/// <param name="userManager"></param>
/// <param name="signInManager"></param>
/// <param name="logger"></param>
public class PersonResourceOwnerPasswordValidator(NaturalPersonManager userManager, SignInManager<NaturalPerson> signInManager, ILogger<PersonResourceOwnerPasswordValidator> logger) 
    : ResourceOwnerPasswordValidator<NaturalPerson>(userManager, signInManager, logger)
{
    public override async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await userManager.FindByNameAsync(context.UserName);
        if (user != null)
        {
            //todo 密码有效性验证需要引入选项。
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
