using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.New;

[BindProperties]
public class ConfidentialModel : PageModel
{
    public string ClientId { get; set; } = default!;

    public string ClientName { get; set; } = default!;

    public string ClientSecret { get; set; } = default!;

    public string SigninCallbackUri { get; set; } = default!;

    public void OnGet()
    {
        ClientId = Guid.NewGuid().ToString();
    }

    public IActionResult OnPost()
    {
        //todo create client and stored to database.
        if (!ModelState.IsValid)
            return Page();


        return RedirectToPage("../Detail/Index", new { anchor = ClientId });
    }
}