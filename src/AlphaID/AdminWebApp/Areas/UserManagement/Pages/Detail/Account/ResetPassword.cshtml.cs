using System.ComponentModel.DataAnnotations;
using System.Text;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class ResetPasswordModel(
    ApplicationUserManager<NaturalPerson> userManager,
    IShortMessageService shortMessageService,
    IOptions<IdentityOptions> options) : PageModel
{
    private const string UpperCharset = @"ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string LowerCharset = @"abcdefghijkmnopqrstuvwxyz";
    private const string NumCharset = @"123456789";
    private const string SymbolCharset = @"!@#$%^&*()_-+={}|[]\:;<>?,./";
    private readonly IdentityOptions _identityOptions = options.Value;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public NaturalPerson Person { get; set; } = null!;

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
            OperationResult = "无法通过短信重置密码。因为用户没有留下移动电话号码。";
            return Page();
        }

        string password = GeneratePassword();
        IdentityResult result = await userManager.AdminResetPasswordAsync(Person, password, true, true);
        if (result.Succeeded)
        {
            await shortMessageService.SendAsync(Person.PhoneNumber, $"您的初始密码是[{password}]（不包括方括号）");
            OperationResult = "密码已重置并告知用户。";
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


        OperationResult = "操作已成功。";
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

        //补全长度
        while (sb.Length < _identityOptions.Password.RequiredLength)
            sb.Append(charset[Random.Shared.Next(charset.Length)]);
        //洗牌
        Shuffle(sb);
        return sb.ToString();
    }

    /// <summary>
    /// 洗牌
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
        public string NewPassword { get; set; } = null!;

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "User must change password on next login")]
        public bool UserMustChangePasswordOnNextLogin { get; set; } = true;

        [Display(Name = "Unlock user (if locked)")]
        public bool UnlockUser { get; set; } = true;
    }
}