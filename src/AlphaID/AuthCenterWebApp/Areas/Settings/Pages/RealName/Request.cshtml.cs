using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName;

public class RequestModel(RealNameRequestManager requestManager) : PageModel
{
    public RealNameRequest Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        RealNameRequest? request = await requestManager.FindByIdAsync(id);
        if (request == null) return NotFound();

        Data = request;
        return Page();
    }

    public async Task<IActionResult> OnGetChineseIdCardImage(int id, string side)
    {
        RealNameRequest? request = await requestManager.FindByIdAsync(id);
        if (request == null) return NotFound();

        if (request is not ChineseIdCardRealNameRequest chineseIdCardRealNameRequest)
            return NotFound();

        return side switch
        {
            "personal" => File(chineseIdCardRealNameRequest.PersonalSide.Data,
                chineseIdCardRealNameRequest.PersonalSide.MimeType),
            "issuer" => File(chineseIdCardRealNameRequest.IssuerSide.Data,
                chineseIdCardRealNameRequest.IssuerSide.MimeType),
            _ => NotFound()
        };
    }
}