using AlphaIDPlatform.Platform;
using IDSubjects;
using IDSubjects.ChineseName;
using IDSubjects.RealName;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace AdminWebApp.Areas.People.Pages.Register;

public class RegisterByChineseIdCardModel : PageModel
{
    private readonly IChineseIdCardOcrService ocrService;
    private readonly NaturalPersonManager personManager;
    private readonly ChinesePersonNameFactory chinesePersonNameFactory;
    private readonly ChineseIdCardManager chineseIdCardManager;
    private readonly ILogger<RegisterByChineseIdCardModel> logger;

    public RegisterByChineseIdCardModel(IChineseIdCardOcrService ocrService, NaturalPersonManager personManager, ChinesePersonNameFactory chinesePersonNameFactory, ChineseIdCardManager realNameValidator, ILogger<RegisterByChineseIdCardModel> logger)
    {
        this.ocrService = ocrService;
        this.personManager = personManager;
        this.chinesePersonNameFactory = chinesePersonNameFactory;
        this.chineseIdCardManager = realNameValidator;
        this.logger = logger;
    }

    [BindProperty]
    public string IdCardFrontBase64 { get; set; } = default!;

    [BindProperty]
    public string IdCardBackBase64 { get; set; } = default!;

    [Display(Name = "PhoneNumber phone number")]
    [BindProperty]
    public string Mobile { get; set; } = default!;

    [Display(Name = "Email")]
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

        var (personalFaceMimeType, personalFaceBytes) = await EnsureBase64Image(this.IdCardFrontBase64);
        var (issuerFaceMimeType, issuerFaceBytes) = await EnsureBase64Image(this.IdCardBackBase64);

        ChineseIdCardFrontOcrResult personalFaceResult;
        ChineseIdCardBackOcrResult issuerFaceResult;
        try
        {
            using var personalFaceStream = new MemoryStream(personalFaceBytes);
            personalFaceResult = await this.ocrService.RecognizeIdCardFront(personalFaceStream);
            using var issuerFaceStream = new MemoryStream(issuerFaceBytes);
            issuerFaceResult = await this.ocrService.RecognizeIdCardBack(issuerFaceStream);
        }
        catch (ChineseIdCardOcrException ex)
        {
            this.logger.LogWarning(ex, "�û��ṩ��ͼ���޷�ʶ��Ϊ��Ч�����֤����");
            this.ModelState.AddModelError("", "�޷�ʶ��Ϊ��Ч�����֤ͼƬ");
            return this.Page();
        }


        //check if exists in database
        var cardExists = this.chineseIdCardManager.Validations.Any(p => p.ChineseIDCard!.CardNumber == personalFaceResult.IdCardNumber);
        if (cardExists)
        {
            this.ModelState.AddModelError("", "�����֤�Ѿ�ע�����");
            return this.Page();
        }

        var exists = this.personManager.Users.Where(p => p.PhoneNumber == phoneNumber.ToString()
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
                                     personalFaceResult.SexString == "��" ? Gender.Male : Gender.Female,
                                     personalFaceResult.Nationality,
                                     personalFaceResult.DateOfBirth,
                                     personalFaceResult.Address,
                                     personalFaceResult.IdCardNumber,
                                     issuerFaceResult.Issuer,
                                     issuerFaceResult.IssueDate,
                                     issuerFaceResult.ExpiresDate);


        PersonBuilder builder = new(userName, new PersonNameInfo(chinesePersonName.FullName, chinesePersonName.Surname, chinesePersonName.GivenName));
        builder.UseChinesePersonName(chinesePersonName);
        builder.SetMobile(phoneNumber, true);
        if (!string.IsNullOrEmpty(this.Email))
            builder.SetEmail(this.Email);

        var result = await this.personManager.CreateAsync(builder.Build());
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error.Description);
            }
            return this.Page();
        }

        //add chinese card
        ChineseIdCardValidation card = new(new ChineseIDCardImage(personalFaceBytes, personalFaceMimeType, issuerFaceBytes, issuerFaceMimeType));
        card.TryApplyChineseIdCardInfo(cardInfo);
        card.TryApplyChinesePersonName(chinesePersonName);
        await this.chineseIdCardManager.CommitAsync(builder.Build(), card);
        await this.chineseIdCardManager.ValidateAsync(card, "System", true);

        return this.RedirectToPage("../Detail/Index", new { id = builder.Build().Id });
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:��֤ƽ̨������", Justification = "<����>")]
    private static Task<(string issuerFaceMimeType, byte[] issuerFaceBytes)> EnsureBase64Image(string iDCardBackBase64)
    {
        var bytes = Convert.FromBase64String(iDCardBackBase64);
        using MemoryStream stream = new(bytes);
        using Image image = Image.FromStream(stream);
        string mime = image.RawFormat.Equals(ImageFormat.Jpeg)
            ? "image/jpeg"
            : image.RawFormat.Equals(ImageFormat.Png)
                ? "image/png"
                : image.RawFormat.Equals(ImageFormat.Bmp) ? "image/bmp" : throw new ArgumentException("��֧�ֵ�ͼ���ʽ");
        return Task.FromResult((mime, bytes));
    }

}
