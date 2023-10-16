using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Profile.Pages.RealName;

public class ConfirmChineseIDCardModel : PageModel
{
    private readonly NaturalPersonManager personManager;
    private readonly ChineseIDCardManager realNameValidator;
    private readonly ChinesePersonNameFactory chinesePersonNameFactory;

    public ConfirmChineseIDCardModel(NaturalPersonManager personManager, ChineseIDCardManager realNameValidator, ChinesePersonNameFactory chinesePersonNameFactory)
    {
        this.personManager = personManager;
        this.realNameValidator = realNameValidator;
        this.chinesePersonNameFactory = chinesePersonNameFactory;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await this.personManager.GetUserAsync(this.HttpContext.User);
        if (person == null)
            return this.BadRequest();

        var pending = await this.realNameValidator.GetPendingRequestAsync(person);
        if (pending == null)
            return this.BadRequest();

        if (pending.ChineseIDCard == null) //当无法图像无法识别时，也不能进行确认。
            return this.BadRequest();

        var chineseName = this.chinesePersonNameFactory.Create(pending.ChineseIDCard.Name);
        this.Input = new()
        {
            Surname = chineseName.Surname,
            GivenName = chineseName.GivenName,
            PinyinSurname = chineseName.PhoneticSurname,
            PinyinGivenName = chineseName.PhoneticGivenName,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await this.personManager.GetUserAsync(this.HttpContext.User);
        if (person == null)
            return this.BadRequest();

        var pending = await this.realNameValidator.GetPendingRequestAsync(person);
        if (pending == null)
            return this.BadRequest();

        if (pending.ChineseIDCard == null) //当无法图像无法识别时，也不能进行确认。
            return this.BadRequest();

        var chineseName = this.chinesePersonNameFactory.Create(pending.ChineseIDCard.Name);
        var sameCheck = true;
        if (this.Input.Surname != chineseName.Surname) sameCheck = false;
        if (this.Input.GivenName != chineseName.GivenName) sameCheck = false;
        if (this.Input.PinyinSurname != chineseName.PhoneticSurname) sameCheck = false;
        if (this.Input.PinyinGivenName != chineseName.PhoneticGivenName) sameCheck = false;

        if (sameCheck)
        {
            pending.TryApplyChinesePersonName(chineseName);
            await this.realNameValidator.UpdateAsync(pending);

            //自动审批通过
            await this.realNameValidator.ValidateAsync(pending, "System", true);
            return this.RedirectToPage("RealNameValidateSuccess");
        }
        else
        {
            pending.TryApplyChinesePersonName(new ChinesePersonName(this.Input.Surname,
                                                                    this.Input.GivenName,
                                                                    this.Input.PinyinSurname,
                                                                    this.Input.PinyinGivenName));
            await this.realNameValidator.UpdateAsync(pending);
            return this.RedirectToPage("Index");
        }
    }

    public class InputModel
    {
        [Display(Name = "Surname")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string? Surname { get; set; } = default!;

        [Display(Name = "Given name")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string GivenName { get; set; } = default!;

        [Display(Name = "Phonetic surname")]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        public string? PinyinSurname { get; set; } = default!;

        [Display(Name = "Phonetic given name")]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        public string PinyinGivenName { get; set; } = default!;
    }
}
