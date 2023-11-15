#nullable disable

using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class DownloadPersonalDataModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly ILogger<DownloadPersonalDataModel> logger;

    public DownloadPersonalDataModel(
        NaturalPersonManager userManager,
        ILogger<DownloadPersonalDataModel> logger)
    {
        this.userManager = userManager;
        this.logger = logger;
    }

    public IActionResult OnGet()
    {
        return this.NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        this.logger.LogInformation("User with ID '{UserId}' asked for their personal data.", this.userManager.GetUserId(this.User));

        // Only include personal data for download
        var personalData = new Dictionary<string, string>();
        var personalDataProps = typeof(NaturalPerson).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        foreach (var p in personalDataProps)
        {
            personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        }

        var logins = await this.userManager.GetLoginsAsync(user);
        foreach (var l in logins)
        {
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        }

        personalData.Add("Authenticator Key", await this.userManager.GetAuthenticatorKeyAsync(user));

        this.Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalDataAttribute.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }
}
