using IdSubjects.RealName.Requesting;

namespace AdminWebApp.Areas.RealName.Pages
{
    public class IndexModel : PageModel
    {
        private RealNameRequestManager requestManager;

        public IndexModel(RealNameRequestManager requestManager)
        {
            this.requestManager = requestManager;
        }

        public IEnumerable<RealNameRequest> PendingRequests { get; set; } = Enumerable.Empty<RealNameRequest>();

        public void OnGet()
        {
            this.PendingRequests = this.requestManager.PendingRequests.Take(10);
        }
    }
}
