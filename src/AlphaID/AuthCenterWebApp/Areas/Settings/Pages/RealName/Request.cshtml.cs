using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class RequestModel(RealNameRequestManager requestManager) : PageModel
    {
        public RealNameRequest Data { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var request = await requestManager.FindByIdAsync(id);
            if (request == null)
            {
                return this.NotFound();
            }

            this.Data = request;
            return this.Page();
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
