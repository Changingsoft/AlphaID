#nullable disable

using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly NaturalPersonManager _userManager;
    private readonly SignInManager<NaturalPerson> _signInManager;

    public IndexModel(
        NaturalPersonManager userManager,
        SignInManager<NaturalPerson> signInManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
    }

    [Display(Name = "Current user name")]
    public string Username { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Display(Name = "New user name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string NewUserName { get; set; }
    }

    private async Task LoadAsync(NaturalPerson user)
    {
        var userName = await this._userManager.GetUserNameAsync(user);

        this.Username = userName;

        this.Input = new InputModel
        {
            NewUserName = userName,
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        await this.LoadAsync(user);
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        if (!this.ModelState.IsValid)
        {
            await this.LoadAsync(user);
            return this.Page();
        }

        var userName = await this._userManager.GetUserNameAsync(user);
        if (this.Input.NewUserName != userName)
        {
            var result = await this._userManager.SetUserNameAsync(user, this.Input.NewUserName);
            if (!result.Succeeded)
            {
                this.StatusMessage = "Unexpected error when trying to set phone number.";
                return this.RedirectToPage();
            }
        }

        await this._signInManager.RefreshSignInAsync(user);
        this.StatusMessage = "您的个人资料已更新";
        return this.RedirectToPage();
    }
}
