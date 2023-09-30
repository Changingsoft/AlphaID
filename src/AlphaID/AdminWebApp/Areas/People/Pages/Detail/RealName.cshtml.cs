using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class RealNameModel : PageModel
    {
        private readonly NaturalPersonManager userManager;
        private readonly ChineseIDCardManager chineseIDCardManager;

        public RealNameModel(NaturalPersonManager userManager, ChineseIDCardManager chineseIDCardManager)
        {
            this.userManager = userManager;
            this.chineseIDCardManager = chineseIDCardManager;
        }

        public NaturalPerson Data { get; set; } = default!;
        public ChineseIDCardValidation? Card { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            var person = await this.userManager.FindByIdAsync(id);
            if (person == null)
                return this.NotFound();

            this.Card = await this.chineseIDCardManager.GetCurrentAsync(person);

            this.Data = person;
            return this.Page();
        }
    }
}
