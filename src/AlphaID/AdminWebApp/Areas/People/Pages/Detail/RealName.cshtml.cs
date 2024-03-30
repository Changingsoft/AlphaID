using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class RealNameModel(NaturalPersonManager userManager, RealNameManager realNameManager) : PageModel
    {
        public NaturalPerson Data { get; set; } = default!;

        public IEnumerable<RealNameAuthentication> RealNameAuthentications { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var person = await userManager.FindByIdAsync(anchor);
            if (person == null)
                return NotFound();

            RealNameAuthentications = realNameManager.GetAuthentications(person);

            Data = person;
            return Page();
        }
    }
}
