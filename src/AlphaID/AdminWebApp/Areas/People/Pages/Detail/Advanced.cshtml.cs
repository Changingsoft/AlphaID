using IDSubjects;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> OnGet(string id)
        {
            var person = await this.naturalPersonManager.FindByIdAsync(id);
            if (person == null)
                return this.NotFound();
            this.Data = person;
            return this.Page();
        }
    }
}
