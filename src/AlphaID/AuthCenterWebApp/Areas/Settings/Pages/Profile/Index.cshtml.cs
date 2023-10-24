using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public class InputModel
        {
            [Display(Name = "Bio", Description = "Short description about yourself.")]
            [StringLength(200)]
            public string? Bio { get; set; }
        }
    }
}
