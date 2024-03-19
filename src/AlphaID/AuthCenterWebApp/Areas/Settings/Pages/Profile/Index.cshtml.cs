using AuthCenterWebApp.Services;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AuthCenterWebApp.Areas.Settings.Pages.Profile
{
    public class IndexModel(NaturalPersonManager personManager, PersonSignInManager signInManager) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public NaturalPerson Person { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await personManager.GetUserAsync(this.User);
            this.Person = person ?? throw new InvalidOperationException("无法从登录找到用户信息，请联系系统管理员。");
            this.Input = new InputModel()
            {
                Bio = person.Bio,
                Website = person.WebSite,
                Gender = person.Gender,
                DateOfBirth = person.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var person = await personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);

            if (!this.ModelState.IsValid)
                return this.Page();

            person.Bio = this.Input.Bio;
            person.WebSite = this.Input.Website;
            person.Gender = this.Input.Gender;
            person.DateOfBirth = this.Input.DateOfBirth.HasValue ? DateOnly.FromDateTime(this.Input.DateOfBirth.Value) : null;

            this.Result = await personManager.UpdateAsync(person);
            return this.Page();
        }

        public async Task<ActionResult> OnPostUpdateProfilePictureAsync()
        {
            if (!this.Request.Form.Files.Any())
                return this.BadRequest();

            var file = this.Request.Form.Files[0];
            var person = await personManager.GetUserAsync(this.User);
            if (person == null)
                return this.BadRequest();
            await using var stream = file.OpenReadStream();
            byte[] data = new byte[stream.Length];
            await stream.ReadAsync(data, 0, data.Length);
            var result = await personManager.SetProfilePictureAsync(person, file.ContentType, data);
            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(person);
                return new JsonResult(true);
            }
            else
                return new JsonResult("Can not update profile picture.");
        }

        public async Task<IActionResult> OnPostClearProfilePictureAsync()
        {
            var person = await personManager.GetUserAsync(this.User);
            if (person == null)
                return this.BadRequest();
            this.Result = await personManager.ClearProfilePictureAsync(person);
            if (this.Result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(person);
            }
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Bio", Description = "Short description about yourself.")]
            [StringLength(200, ErrorMessage = "Validate_StringLength")]
            public string? Bio { get; set; }

            [Display(Name = "Website", Description = "Your personal website.")]
            [DataType(DataType.Url)]
            public string? Website { get; set; }

            [Display(Name = "Gender")]
            public Gender? Gender { get; set; }

            [Display(Name = "Birth date")]
            [DataType(DataType.Date)]
            public DateTime? DateOfBirth { get; set; }
        }
    }
}
