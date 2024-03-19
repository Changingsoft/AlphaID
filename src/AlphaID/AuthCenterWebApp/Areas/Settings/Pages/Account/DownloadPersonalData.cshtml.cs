#nullable disable

using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class DownloadPersonalDataModel(
    NaturalPersonManager userManager,
    ILogger<DownloadPersonalDataModel> logger) : PageModel
{
    public IActionResult OnGet()
    {
        return this.NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }

        logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userManager.GetUserId(this.User));

        // Only include personal data for download
        var personalData = new Dictionary<string, string>();
        var personalDataProps = typeof(NaturalPerson).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        foreach (var p in personalDataProps)
        {
            personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        }

        var logins = await userManager.GetLoginsAsync(user);
        foreach (var l in logins)
        {
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        }

        personalData.Add("Authenticator Key", await userManager.GetAuthenticatorKeyAsync(user));

        this.Response.Headers.Append("Content-Disposition", "attachment; filename=PersonalDataAttribute.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }
}
