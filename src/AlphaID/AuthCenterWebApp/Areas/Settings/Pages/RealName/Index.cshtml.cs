using IdSubjects;
using IdSubjects.RealName;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class IndexModel(NaturalPersonManager personManager, RealNameManager realNameManager, RealNameRequestManager realNameRequestManager) : PageModel
    {
        public IEnumerable<RealNameAuthentication> Authentications { get; set; } = default!;

        public IEnumerable<RealNameRequest> Requests { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await personManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.NotFound();
            }

            this.Authentications = realNameManager.GetAuthentications(person);
            this.Requests = realNameRequestManager.GetRequests(person).Where(r => !r.Accepted.HasValue);
            return this.Page();
        }
    }
}
