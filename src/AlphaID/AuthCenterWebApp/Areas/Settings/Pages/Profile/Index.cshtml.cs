using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using AlphaIdPlatform.Identity;
using AuthCenterWebApp.Services;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Profile;

public class IndexModel(ApplicationUserManager<ApplicationUser> personManager, PersonSignInManager signInManager, NaturalPersonService naturalPersonService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public ApplicationUser Person { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? person = await personManager.GetUserAsync(User);
        Person = person ?? throw new InvalidOperationException("无法从登录找到用户信息，请联系系统管理员。");
        Input = new InputModel
        {
            Bio = person.Bio,
            Website = person.WebSite,
            Gender = person.Gender,
            DateOfBirth = person.DateOfBirth?.ToDateTime(TimeOnly.MinValue)
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUser? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);
        Person = person;

        if (!ModelState.IsValid)
            return Page();

        person.Bio = Input.Bio;
        person.WebSite = Input.Website;
        person.Gender = Input.Gender;
        person.DateOfBirth = Input.DateOfBirth.HasValue ? DateOnly.FromDateTime(Input.DateOfBirth.Value) : null;

        Result = await naturalPersonService.UpdateAsync(person);
        return Page();
    }

    public async Task<ActionResult> OnPostUpdateProfilePictureAsync()
    {
        if (!Request.Form.Files.Any())
            return BadRequest();

        IFormFile file = Request.Form.Files[0];
        ApplicationUser? person = await personManager.GetUserAsync(User);
        if (person == null)
            return BadRequest();
        await using Stream stream = file.OpenReadStream();
        var data = new byte[stream.Length];
        await stream.ReadAsync(data);
        IdentityResult result = await personManager.SetProfilePictureAsync(person, file.ContentType, data);
        if (result.Succeeded)
        {
            await signInManager.RefreshSignInAsync(person);
            return new JsonResult(true);
        }

        return new JsonResult("Can not update profile picture.");
    }

    public async Task<IActionResult> OnPostClearProfilePictureAsync()
    {
        ApplicationUser? person = await personManager.GetUserAsync(User);
        if (person == null)
            return BadRequest();
        Result = await personManager.ClearProfilePictureAsync(person);
        if (Result.Succeeded) await signInManager.RefreshSignInAsync(person);
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Bio", Description = "Short description about yourself.")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string? Bio { get; set; }

        [Display(Name = "Website")]
        [DataType(DataType.Url)]
        [StringLength(256, ErrorMessage = "Validate_StringLength")]
        public string? Website { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}