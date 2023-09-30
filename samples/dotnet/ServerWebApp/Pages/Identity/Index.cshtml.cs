using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OIDCTestClientApp.Pages.Identity;

public class IndexModel : PageModel
{

    public string? AccessToken { get; set; }

    public string? IdToken { get; set; }

    public string? RefreshToken { get; set; }

    public async Task OnGetAsync()
    {
        this.AccessToken = await this.HttpContext.GetTokenAsync("access_token");
        this.IdToken = await this.HttpContext.GetTokenAsync("id_token");
        this.RefreshToken = await this.HttpContext.GetTokenAsync("refresh_token");
    }

    public IActionResult OnPost()
    {
        var signedOutUri = this.Url.Page("/Index");

        var authProperties = new AuthenticationProperties()
        {
            RedirectUri = signedOutUri,
            AllowRefresh = true,
        };

        return this.SignOut(authProperties,
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
