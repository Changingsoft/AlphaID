using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AuthCenterWebApp.Areas.Settings.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly NaturalPersonManager personManager;

        public IndexModel(NaturalPersonManager personManager)
        {
            this.personManager = personManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await this.personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);
            this.Input = new InputModel()
            {
                Bio = person.Bio,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var person = await this.personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);

            if (!this.ModelState.IsValid)
                return this.Page();

            person.Bio = this.Input.Bio;

            var result = await this.personManager.UpdateAsync(person);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    this.ModelState.AddModelError("", error.Description);
            }
            return this.Page();
        }

        public async Task<IActionResult> OnPostUpdateProfilePictureAsync()
        {
            if (!this.Request.Form.Files.Any())
                return this.BadRequest();

            var file = this.Request.Form.Files[0];
            var person = await this.personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);
            using var stream = file.OpenReadStream();
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var result = await this.personManager.SetProfilePictureAsync(person, file.ContentType, data);
            if (result.Succeeded)
                return new JsonResult(true);
            else
                return new JsonResult("Can not update profile picture.");
        }

        public class InputModel
        {
            [Display(Name = "Bio", Description = "Short description about yourself.")]
            [StringLength(200)]
            public string? Bio { get; set; }
        }
    }
}
