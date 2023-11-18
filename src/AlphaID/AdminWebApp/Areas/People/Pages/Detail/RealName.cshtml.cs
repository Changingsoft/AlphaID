using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class RealNameModel : PageModel
    {
        private readonly NaturalPersonManager userManager;
        private readonly RealNameManager realNameManager;

        public RealNameModel(NaturalPersonManager userManager, RealNameManager realNameManager)
        {
            this.userManager = userManager;
            this.realNameManager = realNameManager;
        }

        public NaturalPerson Data { get; set; } = default!;

        public RealNameState? RealName { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var person = await this.userManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();

            this.RealName = await this.realNameManager.GetRealNameStateAsync(person);

            this.Data = person;
            return this.Page();
        }
    }
}
