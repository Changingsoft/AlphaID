using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class AdvancedModel : PageModel
    {
        private readonly NaturalPersonManager naturalPersonManager;

        public AdvancedModel(NaturalPersonManager naturalPersonManager)
        {
            this.naturalPersonManager = naturalPersonManager;
        }

        public NaturalPerson Data { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var person = await this.naturalPersonManager.FindByIdAsync(anchor);
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
            var person = await this.naturalPersonManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            this.Data = person;

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Data.Enabled = this.Input.Enabled;

            this.Result = await this.naturalPersonManager.UpdateAsync(person);
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Enabled")]
            public bool Enabled { get; set; }
        }
    }
}
