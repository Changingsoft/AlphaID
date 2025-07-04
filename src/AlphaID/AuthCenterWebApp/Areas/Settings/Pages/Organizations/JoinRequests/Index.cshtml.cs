using System.Diagnostics;
using AlphaIdPlatform.JoinOrgRequesting;
using AlphaIdPlatform.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations.JoinRequests
{
    public class IndexModel(IJoinOrganizationRequestStore store, JoinOrganizationManager manager) : PageModel
    {
        public IEnumerable<JoinOrganizationRequest> PendingRequests { get; set; } = [];

        public bool HasItem { get; set; }

        public void OnGet()
        {
            var userId = User.SubjectId();
            Debug.Assert(userId != null);
            var requests = from request in store.Requests.Pending()
                           where request.UserId == userId
                           orderby request.WhenCreated descending
                           select request;
            HasItem = requests.Any();
            PendingRequests = requests;
        }

        public async Task<IActionResult> OnPostCancelRequest(int id)
        {
            var userId = User.SubjectId();
            Debug.Assert(userId != null);
            var requests = from request in store.Requests.Pending()
                           where request.UserId == userId
                           orderby request.WhenCreated descending
                           select request;
            HasItem = requests.Any();
            PendingRequests = requests;

            var currentRequest = requests.FirstOrDefault(r => r.Id == id);
            if (currentRequest != null)
            {
                await manager.Cancel(currentRequest);
            }

            return RedirectToPage();
        }
    }
}
