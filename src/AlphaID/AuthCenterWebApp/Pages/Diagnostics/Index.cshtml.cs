using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace AuthCenterWebApp.Pages.Diagnostics;

[Authorize]
public class Index(IWebHostEnvironment env) : PageModel
{
    public ViewModel View { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!env.IsDevelopment())
            return this.NotFound();

        this.View = new ViewModel(await this.HttpContext.AuthenticateAsync());

        return this.Page();
    }

    public class ViewModel
    {
        public ViewModel(AuthenticateResult result)
        {
            this.AuthenticateResult = result;
            if (result.Properties == null)
            {
                return;
            }

            if (!result.Properties.Items.ContainsKey("client_list"))
            {
                return;
            }

            var encoded = result.Properties.Items["client_list"];
            if (encoded == null)
            {
                return;
            }

            var bytes = Base64Url.Decode(encoded);
            var value = Encoding.UTF8.GetString(bytes);

            this.Clients = JsonSerializer.Deserialize<string[]>(value)!;
        }

        public AuthenticateResult AuthenticateResult { get; }
        public IEnumerable<string> Clients { get; } = [];
    }
}