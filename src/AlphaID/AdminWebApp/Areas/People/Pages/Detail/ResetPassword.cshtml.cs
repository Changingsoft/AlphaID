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
            this.OperationResult = "无法通过短信重置密码。因为用户没有留下移动电话号码。";
            return this.Page();
        }

        var resetTokenTask = this.userManager.GeneratePasswordResetTokenAsync(this.Person);
        var password = this.GeneratePassword();
        var result = await this.userManager.ResetPasswordAsync(this.Person, await resetTokenTask, password);
        if (result.Succeeded)
        {
            if (await this.userManager.IsLockedOutAsync(this.Person))
                await this.userManager.SetLockoutEndDateAsync(this.Person, DateTime.UtcNow); //unlock user

            //设置用户首次登录必须更改密码。
            this.Person.PasswordLastSet = null; //todo 重置密码后要求用户首次必须修改密码。
            await this.userManager.UpdateAsync(this.Person);

            await this.shortMessageService.SendAsync(this.Person.PhoneNumber, $"您的初始密码是[{password}]（不包括方括号）");
            this.OperationResult = "密码已重置并告知用户。";
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

        this.OperationResult = "操作已成功。";
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
        //补全长度
        while (sb.Length < this.identityOptions.Password.RequiredLength)
        {
            sb.Append(charset[Random.Shared.Next(charset.Length)]);
        }
        //洗牌
        this.Shuffle(sb);
        return sb.ToString();
    }

    /// <summary>
    /// 洗牌
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
