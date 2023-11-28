using IdSubjects.RealName.Requesting;

namespace AdminWebApp.Areas.RealName.Pages
{
    public class RequestsModel : PageModel
    {
        private readonly RealNameRequestManager requestManager;

        public RequestsModel(RealNameRequestManager requestManager)
        {
            this.requestManager = requestManager;
        }

        public IEnumerable<RealNameRequest> RealNameRequests { get; set; } = Enumerable.Empty<RealNameRequest>();

        public void OnGet()
        {
            this.RealNameRequests = this.requestManager.Requests.OrderByDescending(p => p.WhenCommitted).Take(1000);
        }
    }
}
