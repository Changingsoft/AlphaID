using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static Duende.IdentityModel.OidcConstants;

namespace AuthCenterWebApp.Pages.Consent;

[Authorize]
[SecurityHeaders]
public class Index(
    IIdentityServerInteractionService interaction,
    IEventService events,
    ILogger<Index> logger) : PageModel
{
    public ViewModel View { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string returnUrl)
    {
        if (!await SetViewModelAsync(returnUrl)) return RedirectToPage("/Home/Error/Index");

        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // validate return url is still valid
        AuthorizationRequest? request = await interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
        if (request == null) return RedirectToPage("/Home/Error/Index");

        ConsentResponse? grantedConsent = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (Input.Button == "no")
        {
            grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

            // emit event
            await events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId,
                request.ValidatedResources.RawScopeValues));
            Telemetry.Metrics.ConsentDenied(request.Client.ClientId,
                request.ValidatedResources.ParsedScopes.Select(s => s.ParsedName));
        }
        // user clicked 'yes' - validate the data
        else if (Input.Button == "yes")
        {
            // if the user consented to some scope, build the response model
            if (Input.ScopesConsented.Any())
            {
                IEnumerable<string> scopes = Input.ScopesConsented;
                if (ConsentOptions.EnableOfflineAccess == false)
                    scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);

                grantedConsent = new ConsentResponse
                {
                    RememberConsent = Input.RememberConsent,
                    ScopesValuesConsented = scopes.ToArray(),
                    Description = Input.Description
                };

                // emit event
                await events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId,
                    request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented,
                    grantedConsent.RememberConsent));
                Telemetry.Metrics.ConsentGranted(request.Client.ClientId, grantedConsent.ScopesValuesConsented,
                    grantedConsent.RememberConsent);
                IEnumerable<string> denied = request.ValidatedResources.ParsedScopes.Select(s => s.ParsedName)
                    .Except(grantedConsent.ScopesValuesConsented);
                Telemetry.Metrics.ConsentDenied(request.Client.ClientId, denied);
            }
            else
            {
                ModelState.AddModelError("", ConsentOptions.MustChooseOneErrorMessage);
            }
        }
        else
        {
            ModelState.AddModelError("", ConsentOptions.InvalidSelectionErrorMessage);
        }

        if (grantedConsent != null)
        {
            ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));

            // communicate outcome of consent back to identity server
            await interaction.GrantConsentAsync(request, grantedConsent);

            // redirect back to authorization endpoint
            if (request.IsNativeClient())
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(Input.ReturnUrl);

            return Redirect(Input.ReturnUrl);
        }

        // we need to redisplay the consent UI
        if (!await SetViewModelAsync(Input.ReturnUrl)) return RedirectToPage("/Home/Error/Index");
        return Page();
    }

    private async Task<bool> SetViewModelAsync(string? returnUrl)
    {
        ArgumentNullException.ThrowIfNull(returnUrl);

        AuthorizationRequest? request = await interaction.GetAuthorizationContextAsync(returnUrl);
        if (request != null)
        {
            View = CreateConsentViewModel(request);
            return true;
        }

        logger.NoConsentMatchingRequest(returnUrl);
        return false;
    }

    private ViewModel CreateConsentViewModel(AuthorizationRequest request)
    {
        var vm = new ViewModel
        {
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent,
            IdentityScopes = request.ValidatedResources.Resources.IdentityResources
                .Select(x => CreateScopeViewModel(x, Input == null || Input.ScopesConsented.Contains(x.Name)))
                .ToArray()
        };

        IEnumerable<string> resourceIndicators =
            request.Parameters.GetValues(AuthorizeRequest.Resource) ?? Enumerable.Empty<string>();
        IEnumerable<ApiResource> apiResources =
            request.ValidatedResources.Resources.ApiResources.Where(x => resourceIndicators.Contains(x.Name));

        var apiScopes = new List<ScopeViewModel>();
        foreach (ParsedScopeValue parsedScope in request.ValidatedResources.ParsedScopes)
        {
            ApiScope? apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                ScopeViewModel scopeVm = CreateScopeViewModel(parsedScope, apiScope,
                    Input == null || Input.ScopesConsented.Contains(parsedScope.RawValue));
                scopeVm.Resources = apiResources.Where(x => x.Scopes.Contains(parsedScope.ParsedName))
                    .Select(x => new ResourceViewModel
                    {
                        Name = x.Name,
                        DisplayName = x.DisplayName ?? x.Name
                    }).ToArray();
                apiScopes.Add(scopeVm);
            }
        }

        if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            apiScopes.Add(CreateOfflineAccessScope(Input == null ||
                                                   Input.ScopesConsented.Contains(IdentityServerConstants.StandardScopes
                                                       .OfflineAccess)));
        vm.ApiScopes = apiScopes;

        return vm;
    }

    private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new ScopeViewModel
        {
            Name = identity.Name,
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };
    }

    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        string displayName = apiScope.DisplayName ?? apiScope.Name;
        if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            displayName += ":" + parsedScopeValue.ParsedParameter;

        return new ScopeViewModel
        {
            Name = parsedScopeValue.ParsedName,
            Value = parsedScopeValue.RawValue,
            DisplayName = displayName,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    private static ScopeViewModel CreateOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Value = IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }

    public class ViewModel
    {
        public string ClientName { get; set; } = null!;
        public string? ClientUrl { get; set; }
        public string? ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; } = null!;
        public IEnumerable<ScopeViewModel> ApiScopes { get; set; } = null!;
    }

    public class InputModel
    {
        public string Button { get; set; } = null!;
        public IEnumerable<string> ScopesConsented { get; set; } = null!;

        [Display(Name = "Remember my decision")]
        public bool RememberConsent { get; set; } = true;

        public string ReturnUrl { get; set; } = null!;

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}