using AlphaIDPlatform.Platform;
using IDSubjects;
using IDSubjects.RealName;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WechatWebLogin;

namespace WechatAuthWebApp.Pages;

public class RegisterModel : PageModel
{
    private readonly WechatLoginSessionManager sessionManager;
    private readonly NaturalPersonManager personManager;
    private readonly IChineseIDCardOCRService idCardOCRService;

    public RegisterModel(WechatLoginSessionManager sessionManager, NaturalPersonManager personManager, IChineseIDCardOCRService idCardOCRService)
    {
        this.sessionManager = sessionManager;
        this.personManager = personManager;
        this.idCardOCRService = idCardOCRService;
    }

    [BindProperty(SupportsGet = true)]
    public string SessionId { get; set; } = default!;

    [BindProperty]
    public string IDCardBackBase64 { get; set; } = default!;

    [BindProperty]
    public string IDCardFrontBase64 { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var session = await this.sessionManager.FindAsync(this.SessionId);
        return session == null ? this.BadRequest("无效会话") : string.IsNullOrEmpty(session.Mobile) ? this.BadRequest("无效的移动电话号码。") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
            return this.Page();

        var session = await this.sessionManager.FindAsync(this.SessionId);
        if (session == null)
        {
            this.ModelState.AddModelError("", "无效会话");
            return this.Page();
        }
#if DEBUG
        this.IDCardBackBase64 = "AAAAAA==";
        this.IDCardFrontBase64 = "AAAAAA==";
#endif

        using var idCardBackStream = new MemoryStream(Convert.FromBase64String(this.IDCardBackBase64));
        using var idCardFrontStream = new MemoryStream(Convert.FromBase64String(this.IDCardFrontBase64));

        ChineseIDCardBackOCRResult backResult;
        ChineseIDCardFrontOCRResult frontResult;
        try
        {
            backResult = await this.idCardOCRService.RecognizeIDCardBack(idCardBackStream);
            frontResult = await this.idCardOCRService.RecognizeIDCardFront(idCardFrontStream);
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }
        if (!ChineseIDCardNumber.TryParse(frontResult.IDCardNumber, out ChineseIDCardNumber cardNumber))
        {
            this.ModelState.AddModelError("", "身份证无效");
            return this.Page();
        }

        if (!MobilePhoneNumber.TryParse(session.Mobile, out MobilePhoneNumber mobilePhoneNumber))
        {
            this.ModelState.AddModelError("", "移动电话号码无效");
            return this.Page();
        }

        var card = new ChineseIDCardInfo(frontResult.Name,
                                     Enum.Parse<Sex>(frontResult.SexString),
                                     frontResult.Nationality,
                                     frontResult.DateOfBirth,
                                     frontResult.Address,
                                     frontResult.IDCardNumber,
                                     backResult.Issuer,
                                     backResult.IssueDate,
                                     backResult.ExpiresDate);
        //var factory = new PersonFactory();

        var person = default(NaturalPerson);

        try
        {
            await this.personManager.CreateAsync(person!);

            await this.sessionManager.BindingPersonAsync(session.Id, person!.Id);

            return this.RedirectToPage("/RegisterSuccess", new { sessionid = session.Id });
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }

    }

    public class RegisiterFormData
    {



    }
}
