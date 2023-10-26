using AlphaIDPlatform.Platform;
using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class ResetPasswordModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly IShortMessageService shortMessageService;
    private readonly IdentityOptions identityOptions;

    public ResetPasswordModel(NaturalPersonManager userManager, IShortMessageService shortMessageService, IOptions<IdentityOptions> options)
    {
        this.userManager = userManager;
        this.shortMessageService = shortMessageService;
        this.identityOptions = options.Value;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public NaturalPerson Person { get; set; } = default!;

    public string? OperationResult { get; set; }


    public async Task<IActionResult> OnGet(string anchor)
    {
        var person = await this.userManager.FindByIdAsync(anchor);
        if (person == null)
        {
            return this.NotFound();
        }
        this.Person = person;
        this.Input = new();
        return this.Page();
    }

    public async Task<IActionResult> OnPostAutoReset(string anchor)
    {
        var person = await this.userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        this.Person = person;

        if (string.IsNullOrEmpty(this.Person.PhoneNumber))
        {
            this.OperationResult = "�޷�ͨ�������������롣��Ϊ�û�û�������ƶ��绰���롣";
            return this.Page();
        }

        var resetTokenTask = this.userManager.GeneratePasswordResetTokenAsync(this.Person);
        var password = this.GeneratePassword();
        var result = await this.userManager.ResetPasswordAsync(this.Person, await resetTokenTask, password);
        if (result.Succeeded)
        {
            if (await this.userManager.IsLockedOutAsync(this.Person))
                await this.userManager.SetLockoutEndDateAsync(this.Person, DateTime.UtcNow); //unlock user

            //�����û��״ε�¼����������롣
            this.Person.PasswordLastSet = null; //todo ���������Ҫ���û��״α����޸����롣
            await this.userManager.UpdateAsync(this.Person);

            await this.shortMessageService.SendAsync(this.Person.PhoneNumber, $"���ĳ�ʼ������[{password}]�������������ţ�");
            this.OperationResult = "���������ò���֪�û���";
            return this.Page();
        }
        else
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error.Description);
            }
            return this.Page();
        }
    }

    public async Task<IActionResult> OnPostManualReset(string anchor)
    {
        var person = await this.userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;

        var resetTokenTask = this.userManager.GeneratePasswordResetTokenAsync(this.Person);
        var result = await this.userManager.ResetPasswordAsync(this.Person, await resetTokenTask, this.Input.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error.Description);
            }
            return this.Page();
        }
        if (this.Input.UnlockUser)
        {
            if (await this.userManager.IsLockedOutAsync(this.Person))
                await this.userManager.SetLockoutEndDateAsync(this.Person, DateTime.UtcNow);
        }
        this.Person.PasswordLastSet = this.Input.UserMustChangePasswordOnNextLogin ? null : DateTime.UtcNow;
        await this.userManager.UpdateAsync(this.Person);

        this.OperationResult = "�����ѳɹ���";
        return this.Page();
    }

    private string GeneratePassword()
    {
        var sb = new StringBuilder();
        var charset = new StringBuilder();
        if (this.identityOptions.Password.RequireLowercase)
        {
            sb.Append(LOWER_CHARSET[Random.Shared.Next(LOWER_CHARSET.Length)]);
            charset.Append(LOWER_CHARSET);
        }
        if (this.identityOptions.Password.RequireUppercase)
        {
            sb.Append(UPPER_CHARSET[Random.Shared.Next(UPPER_CHARSET.Length)]);
            charset.Append(UPPER_CHARSET);
        }
        if (this.identityOptions.Password.RequireDigit)
        {
            sb.Append(NUM_CHARSET[Random.Shared.Next(NUM_CHARSET.Length)]);
            charset.Append(NUM_CHARSET);
        }
        if (this.identityOptions.Password.RequireNonAlphanumeric)
        {
            sb.Append(SYMBOL_CHARSET[Random.Shared.Next(SYMBOL_CHARSET.Length)]);
            charset.Append(SYMBOL_CHARSET);
        }
        //��ȫ����
        while (sb.Length < this.identityOptions.Password.RequiredLength)
        {
            sb.Append(charset[Random.Shared.Next(charset.Length)]);
        }
        //ϴ��
        this.Shuffle(sb);
        return sb.ToString();
    }

    /// <summary>
    /// ϴ��
    /// </summary>
    /// <param name="sb"></param>
    /// <returns></returns>
    private void Shuffle(StringBuilder sb)
    {
        for (int i = 0; i < sb.Length - 1; i++)
        {
            var p = i + 1 + Random.Shared.Next(sb.Length - i - 1);
            //swap
            (sb[p], sb[i]) = (sb[i], sb[p]);
        }
    }

    private const string UPPER_CHARSET = @"ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string LOWER_CHARSET = @"abcdefghijkmnopqrstuvwxyz";
    private const string NUM_CHARSET = @"123456789";
    private const string SYMBOL_CHARSET = @"!@#$%^&*()_-+={}|[]\:;<>?,./";

    public class InputModel
    {
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        public string NewPassword { get; set; } = default!;

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; } = default!;

        [Display(Name = "User must change password on next login")]
        public bool UserMustChangePasswordOnNextLogin { get; set; } = true;

        [Display(Name = "Unlock user (if locked)")]
        public bool UnlockUser { get; set; } = true;
    }
}
