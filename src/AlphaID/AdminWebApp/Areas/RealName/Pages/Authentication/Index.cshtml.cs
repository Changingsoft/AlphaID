using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.RealName.Pages.Authentication
{
    public class IndexModel(RealNameManager realNameManager) : PageModel
    {
        public RealNameAuthentication Data { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var authentication = await realNameManager.FindByIdAsync(anchor);
            if (authentication == null)
            {
                return this.NotFound();
            }
            this.Data = authentication;
            return this.Page();
        }
    }
}
