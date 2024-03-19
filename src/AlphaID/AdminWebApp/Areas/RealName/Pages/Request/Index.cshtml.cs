using AlphaIdPlatform.Security;
using IdSubjects;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.RealName.Pages.Request
{
    public class IndexModel(RealNameRequestManager requestManager) : PageModel
    {
        public RealNameRequest Data { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(int anchor)
        {
            var request = await requestManager.FindByIdAsync(anchor);
            if (request == null)
            {
                return this.NotFound();
            }

            this.Data = request;
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int anchor, string button)
        {
            var request = await requestManager.FindByIdAsync(anchor);
            if (request == null)
            {
                return this.NotFound();
            }

            this.Data = request;

            if (!this.ModelState.IsValid)
                return this.Page();

            if (button == "accept")
            {
                this.Result = await requestManager.AcceptAsync(request, this.User.DisplayName());
                
            }
            else
            {
                this.Result = await requestManager.RefuseAsync(request, this.User.DisplayName());
            }

            if (!this.Result.Succeeded)
            {
                return this.Page();
            }

            //todo 继续审核下一个？
            return this.RedirectToPage("/Requests");
        }

        public async Task<IActionResult> OnGetChineseIdCardImage(int id, string side)
        {
            var request = await requestManager.FindByIdAsync(id);
            if (request == null)
            {
                return this.NotFound();
            }

            if (request is not ChineseIdCardRealNameRequest chineseIdCardRealNameRequest)
                return this.NotFound();

            return side switch
            {
                "personal" => this.File(chineseIdCardRealNameRequest.PersonalSide.Data,
                    chineseIdCardRealNameRequest.PersonalSide.MimeType),
                "issuer" => this.File(chineseIdCardRealNameRequest.IssuerSide.Data,
                    chineseIdCardRealNameRequest.IssuerSide.MimeType),
                _ => this.NotFound(),
            };
        }
    }
}
