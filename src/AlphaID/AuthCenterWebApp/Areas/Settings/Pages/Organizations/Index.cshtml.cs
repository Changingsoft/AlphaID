using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations
{
    public class IndexModel(OrganizationMemberManager memberManager, NaturalPersonManager personManager) : PageModel
    {
        public IEnumerable<OrganizationMember> Members { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);

            this.Members = await memberManager.GetMembersOfAsync(person);
            return this.Page();
        }

        public async Task<IActionResult> OnPostLeaveAsync(string organizationId)
        {
            var person = await personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);
            this.Members = await memberManager.GetMembersOfAsync(person);
            var member = this.Members.FirstOrDefault(m => m.OrganizationId == organizationId);
            if (member == null)
                return this.Page();

            this.Result = await memberManager.LeaveOrganizationAsync(member);
            return this.Page();
        }
    }
}
