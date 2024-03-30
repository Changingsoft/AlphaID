using AlphaIdPlatform.Identity;
using AuthCenterWebApp.Services;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class LoginModel(
    IIdentityServerInteractionService interaction,
    IAuthenticationSchemeProvider schemeProvider,
    IIdentityProviderStore identityProviderStore,
    IEventService events,
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager,
    ILogger<LoginModel>? logger) : PageModel
{
    public ViewModel View { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string? returnUrl)
    {
        await BuildModelAsync(returnUrl);
        if (View.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            logger?.LogInformation("we only have one option for logging in and it's an external provider");
            return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, schemeDisplayName = View.ExternalLoginDisplayName, returnUrl });
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        //��������Ƿ�����Ȩ�������������
        var context = await interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        //�û�����ˡ�ȡ������ť
        if (Input.Button != "login")
        {
            if (context != null)
            {
                // ���û���ȡ�����ͻ� IdentityServer���Ա����ܾܾ� consent������ͻ��˲���Ҫconsent����
                // �⽫��ѷ��ʱ��ܾ�����Ӧ���ͻؿͻ��ˡ�
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back access denied OIDC error response to the client.
                await interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl!);
                }

                return Redirect(Input.ReturnUrl!);
            }

            //��������û����Ч�������ģ���ô����ֻ�践����ҳ
            return Redirect("~/");
        }

        if (ModelState.IsValid)
        {
            //��¼���̡�
            var user = await userManager.FindByEmailAsync(Input.Username)
                ?? await userManager.FindByMobileAsync(Input.Username, HttpContext.RequestAborted)
                ?? await userManager.FindByNameAsync(Input.Username);
            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage(Input.ReturnUrl!);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(Input.ReturnUrl!);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }

                    if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    }

                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }

                if (result.MustChangePassword())
                {
                    var principal = GenerateMustChangePasswordPrincipal(user);
                    await signInManager.SignOutAsync();
                    await HttpContext.SignInAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme, principal);
                    return RedirectToPage("ChangePassword", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });
                }

                if (result.IsLockedOut)
                {
                    //_logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
            }


            await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
            ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
        }

        // something went wrong, show form with error
        await BuildModelAsync(Input.ReturnUrl);
        return Page();
    }

    private async Task BuildModelAsync(string? returnUrl)
    {
        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        //��ȡ��Ȩ�����ġ�
        var context = await interaction.GetAuthorizationContextAsync(returnUrl);

        //����Ȩ��������ָ����IdP������������ص�¼��ֱ��ת��ָ����IdP��
        if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;
            var scheme = await schemeProvider.GetSchemeAsync(context.IdP);
            // this is meant to short circuit the UI and only trigger the one external IdP
            View = new ViewModel
            {
                EnableLocalLogin = local,
            };

            Input.Username = context.LoginHint ?? "";

            if (!local)
            {
                View.ExternalProviders =
                [
                    new()
                    {
                        AuthenticationScheme = context.IdP,
                        DisplayName = scheme!.DisplayName!,
                    }
                ];
            }

            return;
        }

        var schemes = await schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        //�Ӵ洢������֤������
        var dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName ?? "",
            });
        providers.AddRange(dynamicSchemes);


        var allowLocal = true;
        var client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin; //�ͻ����Ƿ������ص�¼
            if (client.IdentityProviderRestrictions.Count != 0)
            {
                //����ֻ�пͻ�������ĵ�¼�ṩ����
                providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
            }
        }

        View = new ViewModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = [.. providers]
        };
    }

    private static ClaimsPrincipal GenerateMustChangePasswordPrincipal(NaturalPerson person)
    {
        var identity = new ClaimsIdentity(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, person.Id));
        return new ClaimsPrincipal(identity);
    }
    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "User name", Prompt = "Account name, email, mobile phone number, ID card number, etc.")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me on this device")]
        public bool RememberLogin { get; set; }

        public string? ReturnUrl { get; set; }

        public string Button { get; set; } = default!;
    }

    public class ViewModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = [];
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders.Count() == 1;
        public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.AuthenticationScheme : null;

        public string? ExternalLoginDisplayName => IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.DisplayName : null;

        public class ExternalProvider
        {
            public string DisplayName { get; set; } = default!;
            public string AuthenticationScheme { get; set; } = default!;
        }
    }

    /// <summary>
    /// ��¼ѡ�
    /// </summary>
    public class LoginOptions
    {
        public static readonly bool AllowLocalLogin = true;
        public static readonly bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static readonly string InvalidCredentialsErrorMessage = "�û�����������Ч";
    }
}