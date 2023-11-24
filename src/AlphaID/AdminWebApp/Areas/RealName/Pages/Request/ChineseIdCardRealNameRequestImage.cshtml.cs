using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.RealName.Pages.Request
{
    public class ChineseIdCardRealNameRequestImageModel : PageModel
    {
        private readonly RealNameRequestManager realNameRequestManager;

        public ChineseIdCardRealNameRequestImageModel(RealNameRequestManager realNameRequestManager)
        {
            this.realNameRequestManager = realNameRequestManager;
        }

        public async Task<IActionResult> OnGet(int anchor, string side)
        {
            var request = await this.realNameRequestManager.FindByIdAsync(anchor);
            if (request == null)
            {
                return this.NotFound();
            }

            if (request is not ChineseIdCardRealNameRequest chineseIdCardRealNameRequest)
                return this.NotFound();

            var info = side switch
            {
                "personal" => chineseIdCardRealNameRequest.PersonalSide,
                "issuer" => chineseIdCardRealNameRequest.IssuerSide,
                _ => null,
            };
            return info != null ? this.File(info.Data, info.MimeType, side) : this.NotFound();
        }
    }
}
