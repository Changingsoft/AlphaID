using IDSubjects.ChineseName;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.RealNameValid;

public class ValidateChineseIdCardModel : PageModel
{
    private readonly ChineseIdCardManager realNameValidator;
    private readonly ChinesePersonNameFactory chinesePersonNameFactory;
    private readonly ILogger<ValidateChineseIdCardModel> logger;

    public ValidateChineseIdCardModel(ChineseIdCardManager realNameValidator, ChinesePersonNameFactory chinesePersonNameFactory, ILogger<ValidateChineseIdCardModel> logger)
    {
        this.realNameValidator = realNameValidator;
        this.chinesePersonNameFactory = chinesePersonNameFactory;
        this.logger = logger;
    }

    public ChineseIdCardValidation Data { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGet(int id)
    {
        var data = await this.realNameValidator.FindByIdAsync(id);
        if (data == null)
            return this.NotFound();
        if (data.ChineseIdCard == null)
            return this.NotFound();

        this.Data = data;
        if (this.Data.ChinesePersonName != null)
        {
            this.Input = new()
            {
                Surname = this.Data.ChinesePersonName.Surname,
                GivenName = this.Data.ChinesePersonName.GivenName,
                PinyinSurname = this.Data.ChinesePersonName.PhoneticSurname,
                PinyinGivenName = this.Data.ChinesePersonName.PhoneticGivenName,
            };
        }
        else
        {
            var chineseName = this.chinesePersonNameFactory.Create(this.Data.ChineseIdCard.Name);
            this.Input = new()
            {
                Surname = chineseName.Surname,
                GivenName = chineseName.GivenName,
                PinyinSurname = chineseName.PhoneticSurname,
                PinyinGivenName = chineseName.PhoneticGivenName,
            };
        }
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var data = await this.realNameValidator.FindByIdAsync(id);
        if (data == null)
            return this.NotFound();
        if (data.ChineseIdCard == null)
            return this.NotFound();

        this.Data = data;

        if (!this.ModelState.IsValid)
            return this.Page();

        this.Data.TryApplyChinesePersonName(new(this.Input.Surname, this.Input.GivenName, this.Input.PinyinSurname, this.Input.PinyinGivenName));

        try
        {
            await this.realNameValidator.ValidateAsync(this.Data, this.User.Identity?.Name ?? "[]", this.Input.Accepted);
            return this.RedirectToPage("ValidateRealNameSuccess");
        }
        catch (Exception ex)
        {
            this.logger.LogWarning("An exception {exceptionType} was occurred. message is : {message}", ex.GetType().Name, ex.Message);
            this.ModelState.AddModelError("", ex, this.MetadataProvider.GetMetadataForType(this.Input.GetType()));
            return this.Page();
        }
    }

    public class InputModel
    {
        [Display(Name = "Accepted")]
        public bool Accepted { get; set; }

        [Display(Name = "Surname")]
        public string? Surname { get; set; }

        [Display(Name = "Given name")]
        public string GivenName { get; set; } = default!;

        [Display(Name = "Phonetic surname")]
        public string? PinyinSurname { get; set; }

        [Display(Name = "Phonetic given name")]
        public string PinyinGivenName { get; set; } = default!;
    }
}
