using AlphaIdPlatform.Platform;
using IdSubjects;
using IdSubjects.RealName;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class RequestWithChineseIdCardModel : PageModel
    {
        private readonly NaturalPersonManager naturalPersonManager;
        private readonly IChineseIdCardOcrService chineseIdCardOcrService;
        private readonly RealNameRequestManager realNameRequestManager;

        public RequestWithChineseIdCardModel(IChineseIdCardOcrService chineseIdCardOcrService, NaturalPersonManager naturalPersonManager, RealNameRequestManager realNameRequestManager)
        {
            this.chineseIdCardOcrService = chineseIdCardOcrService;
            this.naturalPersonManager = naturalPersonManager;
            this.realNameRequestManager = realNameRequestManager;
        }

        [BindProperty]
        [Display(Name = "身份证个人信息面")]
        public IFormFile PersonalSide { get; set; } = default!;

        [BindProperty]
        [Display(Name = "身份证国徽面")]
        public IFormFile IssuerSide { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await this.naturalPersonManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.BadRequest("Can not find person.");
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
                return this.Page();

            var person = await this.naturalPersonManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.BadRequest("Can not find person.");
            }

            var personalSideStream = new MemoryStream();
            await this.PersonalSide.OpenReadStream().CopyToAsync(personalSideStream);
            var personalSideOcr = await this.chineseIdCardOcrService.RecognizeIdCardFront(personalSideStream);
            var personalSideInfo = new BinaryDataInfo(this.PersonalSide.ContentType, personalSideStream.ToArray());

            var issuerSideStream = new MemoryStream();
            await this.IssuerSide.OpenReadStream().CopyToAsync(issuerSideStream);
            var issuerSideOcr = await this.chineseIdCardOcrService.RecognizeIdCardBack(issuerSideStream);
            var issuerSideInfo = new BinaryDataInfo(this.IssuerSide.ContentType, issuerSideStream.ToArray());
            var sex = personalSideOcr.SexString switch
            {
                "男" => Sex.Male,
                "女" => Sex.Female,
                _ => throw new ArgumentOutOfRangeException(nameof(personalSideOcr.SexString))
            };
            
            var request = new ChineseIdCardRealNameRequest(personalSideOcr.Name,
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

            this.Result = await this.realNameRequestManager.CreateAsync(person, request);
            if (this.Result.Succeeded)
                return this.RedirectToPage("Index");

            return this.Page();
        }


    }
}
