using IdSubjects;
using IdSubjects.RealName;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class IndexModel : PageModel
    {
        private readonly NaturalPersonManager personManager;
        private readonly RealNameManager realNameManager;
        private readonly RealNameRequestManager realNameRequestManager;

        public IndexModel(NaturalPersonManager personManager, RealNameManager realNameManager, RealNameRequestManager realNameRequestManager)
        {
            this.personManager = personManager;
            this.realNameManager = realNameManager;
            this.realNameRequestManager = realNameRequestManager;
        }

        public IEnumerable<RealNameAuthentication> Authentications { get; set; } = default!;

        public IEnumerable<RealNameRequest> Requests { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await this.personManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.NotFound();
            }

            this.Authentications = this.realNameManager.GetAuthentications(person);
            this.Requests = this.realNameRequestManager.GetRequests(person).Where(r => !r.Accepted.HasValue);
            return this.Page();
        }
    }
}
