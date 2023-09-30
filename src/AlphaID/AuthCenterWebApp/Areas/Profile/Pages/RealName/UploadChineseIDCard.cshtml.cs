using AlphaIDPlatform.Platform;
using IDSubjects;
using IDSubjects.RealName;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Drawing.Imaging;

namespace AuthCenterWebApp.Areas.Profile.Pages.RealName;

public class UploadChineseIDCardModel : PageModel
{
    private readonly ChineseIDCardManager realNameValidator;
    private readonly NaturalPersonManager naturalPersonManager;
    private readonly IChineseIDCardOCRService cardOCRService;

    public UploadChineseIDCardModel(ChineseIDCardManager realNameValidator, NaturalPersonManager naturalPersonManager, IChineseIDCardOCRService cardOCRService)
    {
        this.realNameValidator = realNameValidator;
        this.naturalPersonManager = naturalPersonManager;
        this.cardOCRService = cardOCRService;
    }

    [BindProperty]
    public string IDCardFrontBase64 { get; set; } = default!;

    [BindProperty]
    public string IDCardBackBase64 { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        //if (this.IDCardFrontBase64 == null || this.IDCardBackBase64 == null)
        //this.ModelState.AddModelError("", "���֤ͼƬ�����������ϴ����������֤ͼƬ��");

        if (!this.ModelState.IsValid)
            return this.Page();

        var person = await this.naturalPersonManager.GetUserAsync(this.HttpContext.User);
        if (person == null)
        {
            return this.BadRequest();
        }

        try
        {
            var (personalFaceMimeType, personalFaceBytes) = await EnsureBase64Image(this.IDCardFrontBase64);
            var (issuerFaceMimeType, issuerFaceBytes) = await EnsureBase64Image(this.IDCardBackBase64);

            var validation = new ChineseIDCardValidation(new ChineseIDCardImage(personalFaceBytes, personalFaceMimeType, issuerFaceBytes, issuerFaceMimeType));

            using var personalFaceStream = new MemoryStream(personalFaceBytes);
            var personalFaceResult = await this.cardOCRService.RecognizeIDCardFront(personalFaceStream);
            using var issuerFaceStream = new MemoryStream(issuerFaceBytes);
            var issuerFaceResult = await this.cardOCRService.RecognizeIDCardBack(issuerFaceStream);

            validation.TryApplyChineseIDCardInfo(new ChineseIDCardInfo(personalFaceResult.Name,
                                                                       personalFaceResult.SexString == "��" ? Sex.Male : Sex.Female,
                                                                       personalFaceResult.Nationality,
                                                                       personalFaceResult.DateOfBirth,
                                                                       personalFaceResult.Address,
                                                                       personalFaceResult.IDCardNumber,
                                                                       issuerFaceResult.Issuer,
                                                                       issuerFaceResult.IssueDate,
                                                                       issuerFaceResult.ExpiresDate));

            await this.realNameValidator.CommitAsync(person, validation);
            return this.RedirectToPage("ConfirmChineseIDCard");
        }
        catch (Exception)
        {
            this.ModelState.AddModelError("", "�޷�ʶ����Ч�����֤ͼƬ��");
        }
        return this.Page();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:��֤ƽ̨������", Justification = "<����>")]
    private static Task<(string issuerFaceMimeType, byte[] issuerFaceBytes)> EnsureBase64Image(string iDCardBackBase64)
    {
        var bytes = Convert.FromBase64String(iDCardBackBase64);
        string mime;
        using MemoryStream stream = new(bytes);
        using Image image = Image.FromStream(stream);
        mime = image.RawFormat.Equals(ImageFormat.Jpeg)
            ? "image/jpeg"
            : image.RawFormat.Equals(ImageFormat.Png)
            ? "image/png"
            : image.RawFormat.Equals(ImageFormat.Bmp) ? "image/bmp" : throw new ArgumentException("��֧�ֵ�ͼ���ʽ");
        return Task.FromResult((mime, bytes));
    }
}
