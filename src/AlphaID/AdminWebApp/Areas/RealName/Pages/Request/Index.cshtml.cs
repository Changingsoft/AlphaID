using AlphaIdPlatform.Security;
using IdSubjects;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace AdminWebApp.Areas.RealName.Pages.Request
{
    public class IndexModel : PageModel
    {
        private RealNameRequestManager requestManager;

        public IndexModel(RealNameRequestManager requestManager)
        {
            this.requestManager = requestManager;
        }

        public RealNameRequest Data { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(int anchor)
        {
            var request = await this.requestManager.FindByIdAsync(anchor);
            if (request == null)
            {
                return this.NotFound();
            }

            this.Data = request;
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int anchor, string button)
        {
            var request = await this.requestManager.FindByIdAsync(anchor);
            if (request == null)
            {
                return this.NotFound();
            }

            this.Data = request;

            if (!this.ModelState.IsValid)
                return this.Page();

            if (button == "accept")
            {
                this.Result = await this.requestManager.AcceptAsync(request, this.User.DisplayName());
                
            }
            else
            {
                this.Result = await this.requestManager.RefuseAsync(request, this.User.DisplayName());
            }

            if (!this.Result.Succeeded)
            {
                return this.Page();
            }

            //todo 继续审核下一个？
            return this.RedirectToPage("/Requests");
        }
    }
}
