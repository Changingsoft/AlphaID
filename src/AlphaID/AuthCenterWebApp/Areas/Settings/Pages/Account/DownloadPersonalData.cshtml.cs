using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class DownloadPersonalDataModel(
    NaturalPersonManager userManager,
    ILogger<DownloadPersonalDataModel> logger) : PageModel
{
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true,
    };

    public IActionResult OnGet()
    {
        return NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userManager.GetUserId(User));

        // Only include personal data for download
        IEnumerable<PropertyInfo> personalDataProps = typeof(NaturalPerson).GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

        Dictionary<string, object?> personalData =
            personalDataProps.ToDictionary(p => p.Name, p => p.GetValue(user));

        IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);
        foreach (UserLoginInfo l in logins)
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);

        string? authenticatorKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (authenticatorKey != null)
            personalData.Add("Authenticator Key", authenticatorKey);

        Response.Headers.Append("Content-Disposition", "attachment; filename=PersonalDataAttribute.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData, serializerOptions), "application/json");
    }
}