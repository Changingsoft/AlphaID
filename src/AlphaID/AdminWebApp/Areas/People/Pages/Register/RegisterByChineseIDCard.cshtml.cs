using AlphaIDPlatform.Platform;
using IDSubjects;
using IDSubjects.RealName;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace AdminWebApp.Areas.People.Pages.Register;

public class RegisterByChineseIDCardModel : PageModel
{
    private readonly IChineseIDCardOCRService ocrService;
    private readonly NaturalPersonManager personManager;
    private readonly ChinesePersonNameFactory chinesePersonNameFactory;
    private readonly ChineseIDCardManager chineseIDCardManager;
    private readonly ILogger<RegisterByChineseIDCardModel> logger;

    public RegisterByChineseIDCardModel(IChineseIDCardOCRService ocrService, NaturalPersonManager personManager, ChinesePersonNameFactory chinesePersonNameFactory, ChineseIDCardManager realNameValidator, ILogger<RegisterByChineseIDCardModel> logger)
    {
        this.ocrService = ocrService;
        this.personManager = personManager;
        this.chinesePersonNameFactory = chinesePersonNameFactory;
        this.chineseIDCardManager = realNameValidator;
        this.logger = logger;
    }

    [BindProperty]
    public string IDCardFrontBase64 { get; set; } = default!;

    [BindProperty]
    public string IDCardBackBase64 { get; set; } = default!;

    [Display(Name = "�ƶ��绰����")]
    [BindProperty]
    public string Mobile { get; set; } = default!;

    [Display(Name = "�����ʼ���ַ")]
    [EmailAddress]
    [BindProperty]
    public string? Email { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!MobilePhoneNumber.TryParse(this.Mobile, out var phoneNumber))
        {
            this.ModelState.AddModelError(nameof(this.Mobile), "�ƶ��绰������Ч");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        var (personalFaceMimeType, personalFaceBytes) = await EnsureBase64Image(this.IDCardFrontBase64);
        var (issuerFaceMimeType, issuerFaceBytes) = await EnsureBase64Image(this.IDCardBackBase64);

        ChineseIDCardFrontOCRResult personalFaceResult;
        ChineseIDCardBackOCRResult issuerFaceResult;
        try
        {
            using var personalFaceStream = new MemoryStream(personalFaceBytes);
            personalFaceResult = await this.ocrService.RecognizeIDCardFront(personalFaceStream);
            using var issuerFaceStream = new MemoryStream(issuerFaceBytes);
            issuerFaceResult = await this.ocrService.RecognizeIDCardBack(issuerFaceStream);
        }
        catch (ChineseIDCardOCRException ex)
        {
            this.logger.LogWarning(ex, "�û��ṩ��ͼ���޷�ʶ��Ϊ��Ч�����֤����");
            this.ModelState.AddModelError("", "�޷�ʶ��Ϊ��Ч�����֤ͼƬ");
            return this.Page();
        }


        //check if exists in database
        var cardExists = this.chineseIDCardManager.Validations.Any(p => p.ChineseIDCard!.CardNumber == personalFaceResult.IDCardNumber);
        if (cardExists)
        {
            this.ModelState.AddModelError("", "�����֤�Ѿ�ע�����");
            return this.Page();
        }

        var exists = this.personManager.Users.Where(p => p.Mobile == phoneNumber.ToString()
                                                         || p.UserName == phoneNumber.PhoneNumber);
        if (!string.IsNullOrEmpty(this.Email))
            exists = exists.Where(p => p.Email == this.Email
                                       || p.UserName == this.Email);

        if (exists.Any())
        {
            this.ModelState.AddModelError("", "�����֤���ֻ��Ż�����ʼ���ַ�Ѿ�ע�����");
            return this.Page();
        }

        //ensure username, use email first, otherwise use phone number part.
        var userName = this.Email ?? phoneNumber.PhoneNumber;

        //ensure chinese person name
        var chinesePersonName = this.chinesePersonNameFactory.Create(personalFaceResult.Name);
        ChineseIDCardInfo cardInfo = new(personalFaceResult.Name,
                                     personalFaceResult.SexString == "��" ? Sex.Male : Sex.Female,
                                     personalFaceResult.Nationality,
                                     personalFaceResult.DateOfBirth,
                                     personalFaceResult.Address,
                                     personalFaceResult.IDCardNumber,
                                     issuerFaceResult.Issuer,
                                     issuerFaceResult.IssueDate,
                                     issuerFaceResult.ExpiresDate);


        PersonBuilder builder = new(userName);
        builder.UseChinesePersonName(chinesePersonName);
        builder.SetMobile(phoneNumber, true);
        if (!string.IsNullOrEmpty(this.Email))
            builder.SetEmail(this.Email);

        var result = await this.personManager.CreateAsync(builder.Person);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error.Description);
            }
            return this.Page();
        }

        //add chinese card
        ChineseIDCardValidation card = new(new ChineseIDCardImage(personalFaceBytes, personalFaceMimeType, issuerFaceBytes, issuerFaceMimeType));
        card.TryApplyChineseIDCardInfo(cardInfo);
        card.TryApplyChinesePersonName(chinesePersonName);
        await this.chineseIDCardManager.CommitAsync(builder.Person, card);
        await this.chineseIDCardManager.ValidateAsync(card, "System", true);

        return this.RedirectToPage("../Detail/Index", new { id = builder.Person.Id });
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
