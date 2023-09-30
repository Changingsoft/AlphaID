using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Diagnostics;

[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    private readonly IWebHostEnvironment env;

    public Index(IWebHostEnvironment env)
    {
        this.env = env;
    }

    public ViewModel View { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (!this.env.IsDevelopment())
            return this.NotFound();

        this.View = new ViewModel(await this.HttpContext.AuthenticateAsync());

        return this.Page();
    }
}