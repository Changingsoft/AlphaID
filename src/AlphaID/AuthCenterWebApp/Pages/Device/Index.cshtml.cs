using AuthCenterWebApp.Pages.Consent;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Device;

[SecurityHeaders]
[Authorize]
public class Index(
    IDeviceFlowInteractionService interaction,
    IEventService eventService,
    ILogger<Index> logger) : PageModel
{
    public ViewModel View { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string? userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode))
        {
            this.View = new ViewModel();
            this.Input = new InputModel();
            return this.Page();
        }

        var view = await this.BuildViewModelAsync(userCode);
        if (view == null)
        {
            this.ModelState.AddModelError("", DeviceOptions.InvalidUserCode);
            this.View = new ViewModel();
            this.Input = new InputModel();
            return this.Page();
        }

        this.View = view;
        this.Input = new InputModel
        {
            UserCode = userCode,
        };

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var request = await interaction.GetAuthorizationContextAsync(this.Input.UserCode);
        if (request == null) return this.RedirectToPage("/Home/Error/LoginModel");

        ConsentResponse grantedConsent = default!;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (this.Input.Button == "no")
        {
            grantedConsent = new ConsentResponse
            {
                Error = AuthorizationError.AccessDenied
            };

            // emit event
            await eventService.RaiseAsync(new ConsentDeniedEvent(this.User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
        }
        // user clicked 'yes' - validate the data
        else if (this.Input.Button == "yes")
        {
            // if the user consented to some scope, build the response model
            if (this.Input.ScopesConsented != null && this.Input.ScopesConsented.Any())
            {
                var scopes = this.Input.ScopesConsented;
                if (ConsentOptions.EnableOfflineAccess == false)
                {
                    scopes = scopes.Where(x => x != Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess);
                }

                grantedConsent = new ConsentResponse
                {
                    RememberConsent = this.Input.RememberConsent,
                    ScopesValuesConsented = scopes.ToArray(),
                    Description = this.Input.Description
                };

                // emit event
                await eventService.RaiseAsync(new ConsentGrantedEvent(this.User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
            }
            else
            {
                this.ModelState.AddModelError("", ConsentOptions.MustChooseOneErrorMessage);
            }
        }
        else
        {
            this.ModelState.AddModelError("", ConsentOptions.InvalidSelectionErrorMessage);
        }

        if (grantedConsent != null)
        {
            // communicate outcome of consent back to identity server
            await interaction.HandleRequestAsync(this.Input.UserCode, grantedConsent);

            // indicate that's it ok to redirect back to authorization endpoint
            return this.RedirectToPage("/Device/Success");
        }

        // we need to redisplay the consent UI
        this.View = (await this.BuildViewModelAsync(this.Input.UserCode, this.Input))!;
        return this.Page();
    }


    private async Task<ViewModel?> BuildViewModelAsync(string userCode, InputModel? model = null)
    {
        var request = await interaction.GetAuthorizationContextAsync(userCode);
        return request != null ? this.CreateConsentViewModel(model, request) : null;
    }

    private ViewModel CreateConsentViewModel(InputModel? model, DeviceFlowAuthorizationRequest request)
    {
        var vm = new ViewModel
        {
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent,
            IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => this.CreateScopeViewModel(x, model == null || model.ScopesConsented?.Contains(x.Name) == true)).ToArray()
        };

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                var scopeVm = this.CreateScopeViewModel(parsedScope, apiScope, model == null || model.ScopesConsented?.Contains(parsedScope.RawValue) == true);
                apiScopes.Add(scopeVm);
            }
        }
        if (DeviceOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
        {
            apiScopes.Add(this.GetOfflineAccessScope(model == null || model.ScopesConsented?.Contains(Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess) == true));
        }
        vm.ApiScopes = apiScopes;

        return vm;
    }

    private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new ScopeViewModel
        {
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
        return new ScopeViewModel
        {
            Value = parsedScopeValue.RawValue,
            // todo: use the parsed scope value in the display?
            DisplayName = apiScope.DisplayName ?? apiScope.Name,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    private ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Value = Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = DeviceOptions.OfflineAccessDisplayName,
            Description = DeviceOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }

    public class InputModel
    {
        public string Button { get; set; } = default!;
        public IEnumerable<string>? ScopesConsented { get; set; }
        public bool RememberConsent { get; set; } = true;
        public string ReturnUrl { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string UserCode { get; set; } = default!;
    }
}