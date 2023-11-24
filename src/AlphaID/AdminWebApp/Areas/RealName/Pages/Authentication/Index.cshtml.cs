using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.RealName.Pages.Authentication
{
    public class IndexModel : PageModel
    {
        RealNameManager realNameManager;

        public IndexModel(RealNameManager realNameManager)
        {
            this.realNameManager = realNameManager;
        }

        public RealNameAuthentication Data { get; set; } = default!;

        public async Task<IActionResult> OnGet(string anchor)
        {
            var authentication = await this.realNameManager.FindByIdAsync(anchor);
            if (authentication == null)
            {
                return this.NotFound();
            }
            this.Data = authentication;
            return this.Page();
        }
    }
}
