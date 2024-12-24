using System.ComponentModel.DataAnnotations;
using System.Text;
using AlphaIdPlatform.Platform;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class ResetPasswordModel(
    NaturalPersonManager userManager,
    IShortMessageService shortMessageService,
    IOptions<IdentityOptions> options) : PageModel
{
    private const string UpperCharset = @"ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string LowerCharset = @"abcdefghijkmnopqrstuvwxyz";
    private const string NumCharset = @"123456789";
    private const string SymbolCharset = @"!@#$%^&*()_-+={}|[]\:;<>?,./";
    private readonly IdentityOptions _identityOptions = options.Value;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public NaturalPerson Person { get; set; } = default!;

    public string? OperationResult { get; set; }


    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();
        Person = person;
        Input = new InputModel();
        return Page();
    }

    public async Task<IActionResult> OnPostAutoReset(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Person = person;

        if (string.IsNullOrEmpty(Person.PhoneNumber))
        {
            OperationResult = "�޷�ͨ�������������롣��Ϊ�û�û�������ƶ��绰���롣";
            return Page();
        }

        string password = GeneratePassword();
        IdentityResult result = await userManager.AdminResetPasswordAsync(Person, password, true, true);
        if (result.Succeeded)
        {
            await shortMessageService.SendAsync(Person.PhoneNumber, $"���ĳ�ʼ������[{password}]�������������ţ�");
            OperationResult = "���������ò���֪�û���";
            return Page();
        }

        foreach (IdentityError error in result.Errors) ModelState.AddModelError("", error.Description);
        return Page();
    }

    public async Task<IActionResult> OnPostManualReset(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;

        IdentityResult result = await userManager.AdminResetPasswordAsync(Person, Input.NewPassword,
            Input.UserMustChangePasswordOnNextLogin, Input.UnlockUser);
        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors) ModelState.AddModelError("", error.Description);
            return Page();
        }


        OperationResult = "�����ѳɹ���";
        return Page();
    }

    private string GeneratePassword()
    {
        var sb = new StringBuilder();
        var charset = new StringBuilder();
        if (_identityOptions.Password.RequireLowercase)
        {
            sb.Append(LowerCharset[Random.Shared.Next(LowerCharset.Length)]);
            charset.Append(LowerCharset);
        }

        if (_identityOptions.Password.RequireUppercase)
        {
            sb.Append(UpperCharset[Random.Shared.Next(UpperCharset.Length)]);
            charset.Append(UpperCharset);
        }

        if (_identityOptions.Password.RequireDigit)
        {
            sb.Append(NumCharset[Random.Shared.Next(NumCharset.Length)]);
            charset.Append(NumCharset);
        }

        if (_identityOptions.Password.RequireNonAlphanumeric)
        {
            sb.Append(SymbolCharset[Random.Shared.Next(SymbolCharset.Length)]);
            charset.Append(SymbolCharset);
        }

        //��ȫ����
        while (sb.Length < _identityOptions.Password.RequiredLength)
            sb.Append(charset[Random.Shared.Next(charset.Length)]);
        //ϴ��
        Shuffle(sb);
        return sb.ToString();
    }

    /// <summary>
    ///     ϴ��
    /// </summary>
    /// <param name="sb"></param>
    /// <returns></returns>
    private static void Shuffle(StringBuilder sb)
    {
        for (var i = 0; i < sb.Length - 1; i++)
        {
            int p = i + 1 + Random.Shared.Next(sb.Length - i - 1);
            //swap
            (sb[p], sb[i]) = (sb[i], sb[p]);
        }
    }

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