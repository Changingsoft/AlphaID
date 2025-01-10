using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Platform;
using IdSubjects;
using IdSubjects.RealName;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName.StartRequest;

public class WithChineseIdCardModel(
    IChineseIdCardOcrService chineseIdCardOcrService,
    NaturalPersonManager naturalPersonManager,
    RealNameRequestManager realNameRequestManager) : PageModel
{
    [BindProperty]
    [Display(Name = "身份证个人信息面")]
    public IFormFile PersonalSide { get; set; } = null!;

    [BindProperty]
    [Display(Name = "身份证国徽面")]
    public IFormFile IssuerSide { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await naturalPersonManager.GetUserAsync(User);
        if (person == null) return BadRequest("Can not find person.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        NaturalPerson? person = await naturalPersonManager.GetUserAsync(User);
        if (person == null) return BadRequest("Can not find person.");

        var personalSideStream = new MemoryStream();
        await PersonalSide.OpenReadStream().CopyToAsync(personalSideStream);
        ChineseIdCardFrontOcrResult personalSideOcr =
            await chineseIdCardOcrService.RecognizeIdCardFront(personalSideStream);
        var personalSideInfo = new BinaryDataInfo(PersonalSide.ContentType, personalSideStream.ToArray());

        var issuerSideStream = new MemoryStream();
        await IssuerSide.OpenReadStream().CopyToAsync(issuerSideStream);
        ChineseIdCardBackOcrResult issuerSideOcr = await chineseIdCardOcrService.RecognizeIdCardBack(issuerSideStream);
        var issuerSideInfo = new BinaryDataInfo(IssuerSide.ContentType, issuerSideStream.ToArray());
        Sex sex = personalSideOcr.SexString switch
        {
            "男" => Sex.Male,
            "女" => Sex.Female,
            _ => throw new ArgumentOutOfRangeException(nameof(personalSideOcr.SexString))
        };

        var request = new ChineseIdCardRealNameRequest(person.Id,
            personalSideOcr.Name,
            sex,
            personalSideOcr.Nationality,
            DateOnly.FromDateTime(personalSideOcr.DateOfBirth),
            personalSideOcr.Address,
            personalSideOcr.IdCardNumber,
            issuerSideOcr.Issuer,
            DateOnly.FromDateTime(issuerSideOcr.IssueDate),
            issuerSideOcr.ExpiresDate.HasValue ? DateOnly.FromDateTime(issuerSideOcr.ExpiresDate.Value) : null,
            personalSideInfo,
            issuerSideInfo);

        Result = await realNameRequestManager.CreateAsync(request);
        if (Result.Succeeded)
            return RedirectToPage("../Index");

        return Page();
    }
}