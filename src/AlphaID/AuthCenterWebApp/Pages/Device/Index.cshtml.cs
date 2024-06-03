using AuthCenterWebApp.Pages.Consent;
using Duende.IdentityServer;
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
    IEventService eventService) : PageModel
{
    public ViewModel View { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string? userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode))
        {
            View = new ViewModel();
            Input = new InputModel();
            return Page();
        }

        ViewModel? view = await BuildViewModelAsync(userCode);
        if (view == null)
        {
            ModelState.AddModelError("", DeviceOptions.InvalidUserCode);
            View = new ViewModel();
            Input = new InputModel();
            return Page();
        }

        View = view;
        Input = new InputModel
        {
            UserCode = userCode
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        DeviceFlowAuthorizationRequest? request = await interaction.GetAuthorizationContextAsync(Input.UserCode ?? throw new ArgumentNullException(nameof(Input.UserCode)));
        if (request == null) return RedirectToPage("/Home/Error/Index");

        ConsentResponse grantedConsent = default!;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (Input.Button == "no")
        {
            grantedConsent = new ConsentResponse
            {
                Error = AuthorizationError.AccessDenied
            };

            // emit event
            await eventService.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId,
                request.ValidatedResources.RawScopeValues));
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
                await eventService.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId,
                    request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented,
                    grantedConsent.RememberConsent));
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
            // communicate outcome of consent back to identity server
            await interaction.HandleRequestAsync(Input.UserCode, grantedConsent);

            // indicate that's it ok to redirect back to authorization endpoint
            return RedirectToPage("/Device/Success");
        }

        // we need to redisplay the consent UI
        View = (await BuildViewModelAsync(Input.UserCode, Input))!;
        return Page();
    }


    private async Task<ViewModel?> BuildViewModelAsync(string userCode, InputModel? model = null)
    {
        DeviceFlowAuthorizationRequest? request = await interaction.GetAuthorizationContextAsync(userCode);
        return request != null ? CreateConsentViewModel(model, request) : null;
    }

    private ViewModel CreateConsentViewModel(InputModel? model, DeviceFlowAuthorizationRequest request)
    {
        var vm = new ViewModel
        {
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent,
            IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x =>
                CreateScopeViewModel(x, model == null || model.ScopesConsented.Contains(x.Name))).ToArray()
        };

        var apiScopes = new List<ScopeViewModel>();
        foreach (ParsedScopeValue parsedScope in request.ValidatedResources.ParsedScopes)
        {
            ApiScope? apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                ScopeViewModel scopeVm = CreateScopeViewModel(parsedScope, apiScope,
                    model == null || model.ScopesConsented?.Contains(parsedScope.RawValue) == true);
                apiScopes.Add(scopeVm);
            }
        }

        if (DeviceOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            apiScopes.Add(GetOfflineAccessScope(model == null ||
                                                model.ScopesConsented?.Contains(IdentityServerConstants.StandardScopes
                                                    .OfflineAccess) == true));
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
            Value = IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = DeviceOptions.OfflineAccessDisplayName,
            Description = DeviceOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }

    public class InputModel
    {
        public string? Button { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; } = [];
        public bool RememberConsent { get; set; } = true;
        public string? ReturnUrl { get; set; }
        public string? Description { get; set; }
        public string? UserCode { get; set; }
    }
}