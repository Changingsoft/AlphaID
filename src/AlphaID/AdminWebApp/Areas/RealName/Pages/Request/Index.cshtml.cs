using AlphaIdPlatform.Security;
using IdSubjects;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.RealName.Pages.Request;

public class IndexModel(RealNameRequestManager requestManager) : PageModel
{
    public RealNameRequest Data { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(int anchor)
    {
        RealNameRequest? request = await requestManager.FindByIdAsync(anchor);
        if (request == null) return NotFound();

        Data = request;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor, string button)
    {
        RealNameRequest? request = await requestManager.FindByIdAsync(anchor);
        if (request == null) return NotFound();

        Data = request;

        if (!ModelState.IsValid)
            return Page();

        if (button == "accept")
            Result = await requestManager.AcceptAsync(request, User.DisplayName());
        else
            Result = await requestManager.RefuseAsync(request, User.DisplayName());

        if (!Result.Succeeded) return Page();

        //todo 继续审核下一个？
        return RedirectToPage("/Requests");
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