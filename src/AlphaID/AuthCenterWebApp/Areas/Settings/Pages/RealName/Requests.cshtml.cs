using IdSubjects;
using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class RequestsModel : PageModel
    {
        private readonly RealNameRequestManager realNameRequestManager;
        private readonly NaturalPersonManager naturalPersonManager;

        public RequestsModel(RealNameRequestManager realNameRequestManager, NaturalPersonManager naturalPersonManager)
        {
            this.realNameRequestManager = realNameRequestManager;
            this.naturalPersonManager = naturalPersonManager;
        }

        public IEnumerable<RealNameRequest> RealNameRequests { get; set; } = Enumerable.Empty<RealNameRequest>();

        public async Task<IActionResult> OnGet()
        {
            var person = await this.naturalPersonManager.GetUserAsync(this.User);
            if (person == null)
                return this.NotFound();
            this.RealNameRequests = this.realNameRequestManager.GetRequests(person);
            return this.Page();
        }
    }
}
