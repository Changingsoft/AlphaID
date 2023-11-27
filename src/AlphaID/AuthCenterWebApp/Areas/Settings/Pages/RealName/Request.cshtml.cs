using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class RequestModel : PageModel
    {
        private readonly RealNameRequestManager requestManager;

        public RequestModel(RealNameRequestManager requestManager)
        {
            this.requestManager = requestManager;
        }

        public RealNameRequest Data { get; set; } = default!;

        public async Task<IActionResult> OnGet(int id)
        {
            var request = await this.requestManager.FindByIdAsync(id);
            if (request == null)
            {
                return this.NotFound();
            }

            this.Data = request;
            return this.Page();
        }
    }
}
