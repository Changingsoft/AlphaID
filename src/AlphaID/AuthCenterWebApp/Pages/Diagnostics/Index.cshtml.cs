using System.Text;
using System.Text.Json;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Diagnostics;

[Authorize]
public class Index(IWebHostEnvironment env) : PageModel
{
    public ViewModel View { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!env.IsDevelopment())
            return NotFound();

        View = new ViewModel(await HttpContext.AuthenticateAsync());

        return Page();
    }

    public class ViewModel
    {
        public ViewModel(AuthenticateResult result)
        {
            AuthenticateResult = result;
            if (result.Properties == null) return;

            if (!result.Properties.Items.ContainsKey("client_list")) return;

            string? encoded = result.Properties.Items["client_list"];
            if (encoded == null) return;

            byte[] bytes = Base64Url.Decode(encoded);
            string value = Encoding.UTF8.GetString(bytes);

            Clients = JsonSerializer.Deserialize<string[]>(value)!;
        }

        public AuthenticateResult AuthenticateResult { get; }
        public IEnumerable<string> Clients { get; } = [];
    }
}