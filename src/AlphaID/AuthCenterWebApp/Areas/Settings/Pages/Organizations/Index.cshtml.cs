using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations
{
    public class IndexModel : PageModel
    {
        NaturalPersonManager personManager;
        readonly OrganizationMemberManager memberManager;

        public IndexModel(OrganizationMemberManager memberManager, NaturalPersonManager personManager)
        {
            this.memberManager = memberManager;
            this.personManager = personManager;
        }

        public IEnumerable<OrganizationMember> Members { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            var person = await this.personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);

            this.Members = await this.memberManager.GetMembersOfAsync(person);
            return this.Page();
        }
    }
}
