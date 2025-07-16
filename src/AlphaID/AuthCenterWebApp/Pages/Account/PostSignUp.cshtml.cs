using AlphaIdPlatform.Identity;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthCenterWebApp.Pages.Account
{
    [AllowAnonymous]
    public class PostSignUpModel(UserManager<NaturalPerson> userManager,
        ILogger<PostSignUpModel> logger,
        SignInManager<NaturalPerson> signInManager,
        IIdentityServerInteractionService interaction) : PageModel
    {
        [BindProperty]
        [Display(Name = "User name")]
        public string? UserName { get; set; }

        [BindProperty]
        [Display(Name = "Display name", Description = "Your fully person name, include family name and given name.")]
        [Required]
        [StringLength(10)]
        public string DisplayName { get; set; } = null!;

        [BindProperty]
        [Display(Name = "Date of birth", Description = "Date of birth cannot be changed after sign up.")]
        [DataType(DataType.Date)]
        [Required]
        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18);

        public AuthenticateResult ExternalLoginResult { get; set; } = null!;


        public async Task<IActionResult> OnGet(string? returnUrl)
        {
            AuthenticateResult preSignUpAuthResult = await HttpContext.AuthenticateAsync(AuthenticationDefaults.PreSignUpScheme);
            if (!preSignUpAuthResult.Succeeded)
            {
                return RedirectToPage("SignInOrSignUp", new { returnUrl });
            }
            ExternalLoginResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            return Page();
        }

        public async Task<IActionResult> OnPost(string? returnUrl)
        {
            AuthenticateResult preSignUpAuthResult = await HttpContext.AuthenticateAsync(AuthenticationDefaults.PreSignUpScheme);
            if (!preSignUpAuthResult.Succeeded)
            {
                return RedirectToPage("SignInOrSignUp", new { returnUrl });
            }
            AuthorizationRequest? context = await interaction.GetAuthorizationContextAsync(returnUrl);


            var userNameSeed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 0;
            var userName = $"u-{userNameSeed}";
            var phoneNumber = preSignUpAuthResult.Principal.FindFirstValue(JwtClaimTypes.PhoneNumber);
            bool isRememberLoginParsed = bool.TryParse(preSignUpAuthResult.Principal.FindFirstValue("remember-login") ?? "false", out bool rememberLogin);
            
            // Create the user in the database
            var user = new NaturalPerson(userName);
            user.Name = DisplayName;
            user.DateOfBirth = DateOfBirth;
            user.PhoneNumber = phoneNumber;
            user.PhoneNumberConfirmed = true; // Phone number is confirmed by the pre-sign-up process

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                logger.LogError("创建用户时出错。错误是：{errors}", string.Join(", ", result.Errors));
                ModelState.AddModelError(string.Empty, "创建用户时出错。请稍后再试。");
                return Page();
            }
            

            ExternalLoginResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            //如果有外部登录信息，则将其绑定到外部登录
            if (ExternalLoginResult.Succeeded)
            {
                //为用户绑定外部登录
                ClaimsPrincipal? externalUser = ExternalLoginResult.Principal;
                Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                    externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                    throw new Exception("Unknown userid");

                string? provider = ExternalLoginResult.Properties.Items[".AuthScheme"];
                string? providerDisplayName = ExternalLoginResult.Properties.Items["schemeDisplayName"];
                string providerUserId = userIdClaim.Value;
                await userManager.AddLoginAsync(user,
                    new UserLoginInfo(provider!, providerUserId, providerDisplayName));

                // this allows us to collect any additional claims or properties
                // for the specific protocols used and store them in the local auth cookie.
                // this is typically used to store data needed for sign out from those protocols.
                var additionalLocalClaims = new List<Claim>();
                var localSignInProps = new AuthenticationProperties();
                CaptureExternalLoginContext(ExternalLoginResult, additionalLocalClaims, localSignInProps);

                await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            }

            // Sign in the user
            await signInManager.SignInAsync(user, rememberLogin);
            // 注销前注册验证方案。
            await HttpContext.SignOutAsync(AuthenticationDefaults.PreSignUpScheme);
            if (context != null)
            {
                if (context.IsNativeClient())
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(returnUrl!);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(returnUrl!);
            }

            // request for a local page
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            if (string.IsNullOrEmpty(returnUrl)) return Redirect("~/");

            // user might have clicked on a malicious link - should be logged
            throw new ArgumentException("invalid return URL");
        }

        private void CaptureExternalLoginContext(AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
            // capture the idp used to log in, so the session knows where the user came from
            localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties!.Items[".AuthScheme"]!));

            // if the external system sent a session id claim, copy it over.
            // so we can use it for single sign-out
            Claim? sid = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null) localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));

            // if the external provider issued an id_token, we'll keep it for sign out
            string? idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
                localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
        }

    }
}
