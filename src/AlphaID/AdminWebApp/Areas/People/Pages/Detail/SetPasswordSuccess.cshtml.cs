using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class SetPasswordSuccessModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    public void OnGet()
    {
    }
}
