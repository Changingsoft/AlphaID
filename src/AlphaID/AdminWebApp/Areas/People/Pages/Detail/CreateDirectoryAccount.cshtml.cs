using DirectoryLogon;
using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class CreateDirectoryAccountModel : PageModel
{
    private readonly DirectoryServiceManager directoryServiceManager;
    private readonly LogonAccountManager logonAccountManager;
    private readonly NaturalPersonManager naturalPersonManager;
    private readonly ChineseIDCardManager chineseIDCardManager;

    public CreateDirectoryAccountModel(DirectoryServiceManager directoryServiceManager, LogonAccountManager logonAccountManager, NaturalPersonManager naturalPersonManager, ChineseIDCardManager chineseIDCardManager)
    {
        this.directoryServiceManager = directoryServiceManager;
        this.logonAccountManager = logonAccountManager;
        this.naturalPersonManager = naturalPersonManager;
        this.chineseIDCardManager = chineseIDCardManager;
    }

    public IEnumerable<DirectoryService> DirectoryServices => this.directoryServiceManager.Services;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var person = await this.naturalPersonManager.FindByIdAsync(id);
        if (person == null)
            return this.NotFound();
        var card = await this.chineseIDCardManager.GetCurrentAsync(person);
        if (card == null)
        {
            this.ModelState.AddModelError("", "必须先通过实名认证，才能创建目录账户");
            return this.Page();
        }

        //准备姓名全拼+身份证后4位
        var accountName = $"{person.PhoneticSurname}{person.PhoneticGivenName}{card.ChineseIDCard!.CardNumber[^4..]}".ToLower();

        //准备有关资料
        this.Input = new()
        {
            SAMAccountName = accountName,
            UPNPart = accountName,
            EntryName = accountName,
            Surname = person.LastName!,
            GivenName = person.FirstName!,
            DisplayName = person.Name,
            PinyinSurname = person.PhoneticSurname,
            PinyinGivenName = person.PhoneticGivenName,
            PinyinDisplayName = person.PhoneticSurname + person.PhoneticGivenName,
            Mobile = person.PhoneNumber!,
            Email = person.Email,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var person = await this.naturalPersonManager.FindByIdAsync(id);
        if (person == null)
            return this.NotFound();

        if (!this.ModelState.IsValid)
            return this.Page();

        var directoryService = this.directoryServiceManager.FindByIdAsync(this.Input.ServiceId);
        if (directoryService == null)
            this.ModelState.AddModelError("", "请选择一个目录服务");

        CreateAccountRequest request = new()
        {
            AccountName = this.Input.EntryName,
            SAMAccountName = this.Input.SAMAccountName,
            UpnLeftPart = this.Input.UPNPart,
            DisplayName = this.Input.DisplayName,
            Surname = this.Input.Surname,
            GivenName = this.Input.GivenName,
            Email = this.Input.Email,
            E164Mobile = this.Input.Mobile,
            ServiceId = this.Input.ServiceId,
            InitPassword = this.Input.NewPassword,
        };
        try
        {
            await this.logonAccountManager.CreateAsync(person, request);
            return this.RedirectToPage("DirectoryAccounts", new { id });
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }
    }

    public class InputModel
    {
        [Display(Name = "Directory Service")]
        public int ServiceId { get; set; }

        [Display(Name = "SAM Account Name")]
        public string SAMAccountName { get; set; } = default!;

        [Display(Name = "UPN Prefix Part")]
        public string UPNPart { get; set; } = default!;

        [Display(Name = "Directory Entry Name")]
        public string EntryName { get; set; } = default!;

        public string Surname { get; set; } = default!;

        public string GivenName { get; set; } = default!;

        public string DisplayName { get; set; } = default!;

        public string? PinyinSurname { get; set; }

        public string? PinyinGivenName { get; set; }

        public string? PinyinDisplayName { get; set; }

        [Display(Name = "PhoneNumber Phone Number")]
        public string Mobile { get; set; } = default!;

        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Display(Name = "Password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = default!;

        [Display(Name = "Confirm Password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
