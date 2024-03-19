using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class AdvancedModel(NaturalPersonManager naturalPersonManager) : PageModel
    {
        public NaturalPerson Data { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var person = await naturalPersonManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            this.Data = person;

            this.Input = new InputModel
            {
                Enabled = this.Data.Enabled,
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var person = await naturalPersonManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            this.Data = person;

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Data.Enabled = this.Input.Enabled;

            this.Result = await naturalPersonManager.UpdateAsync(person);
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Enabled")]
            public bool Enabled { get; set; }
        }
    }
}
