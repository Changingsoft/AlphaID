using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations
{
    public class IndexModel : PageModel
    {
        private readonly NaturalPersonManager personManager;
        private readonly OrganizationMemberManager memberManager;

        public IndexModel(OrganizationMemberManager memberManager, NaturalPersonManager personManager)
        {
            this.memberManager = memberManager;
            this.personManager = personManager;
        }

        public IEnumerable<OrganizationMember> Members { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var person = await this.personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);

            this.Members = await this.memberManager.GetMembersOfAsync(person);
            return this.Page();
        }

        public async Task<IActionResult> OnPostLeaveAsync(string organizationId)
        {
            var person = await this.personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);
            this.Members = await this.memberManager.GetMembersOfAsync(person);
            var member = this.Members.FirstOrDefault(m => m.OrganizationId == organizationId);
            if (member == null)
                return this.Page();

            this.Result = await this.memberManager.LeaveOrganizationAsync(member);
            return this.Page();
        }
    }
}
