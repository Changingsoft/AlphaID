using IdSubjects.RealName.Requesting;

namespace AdminWebApp.Areas.RealName.Pages;

public class IndexModel(RealNameRequestManager requestManager) : PageModel
{
    public IEnumerable<RealNameRequest> PendingRequests { get; set; } = [];

    public void OnGet()
    {
        PendingRequests = requestManager.PendingRequests.Take(10);
    }
}