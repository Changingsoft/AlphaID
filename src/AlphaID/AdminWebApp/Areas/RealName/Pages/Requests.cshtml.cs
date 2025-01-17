using AlphaIdPlatform.Identity;
using IdSubjects.RealName.Requesting;

namespace AdminWebApp.Areas.RealName.Pages;

public class RequestsModel(RealNameRequestManager<NaturalPerson> requestManager) : PageModel
{
    public IEnumerable<RealNameRequest> RealNameRequests { get; set; } = [];

    public void OnGet()
    {
        RealNameRequests = requestManager.Requests.OrderByDescending(p => p.WhenCommitted).Take(1000);
    }
}