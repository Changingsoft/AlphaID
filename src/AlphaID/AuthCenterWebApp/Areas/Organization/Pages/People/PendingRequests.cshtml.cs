using System.Diagnostics;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.JoinOrgRequesting;
using AlphaIdPlatform.Security;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class PendingRequestsModel(
        IOrganizationStore organizationStore,
        IJoinOrganizationRequestStore joinOrganizationRequestStore,
        JoinOrganizationManager joinOrganizationManager,
        IQueryableUserStore<NaturalPerson> userStore) : PageModel
    {
        public IEnumerable<JoinOrgRequestViewModel> PendingRequests { get; set; } = [];

        async Task InitPendingRequests(string orgName)
        {
            var orgId = (from org in organizationStore.Organizations
                         where org.Name == orgName
                         select org.Id).FirstOrDefault();
            if (orgId == null)
                return;

            var requests = (from r in joinOrganizationRequestStore.Requests.Pending()
                            where r.OrganizationId == orgId
                            select r).ToList();
            var userIds = requests.Select(r => r.UserId).Distinct().ToList();
            var users = from user in userStore.Users
                        where userIds.Contains(user.Id)
                        select new { user.Id, user.UserName, user.Name };

            PendingRequests = from req in requests
                              join user in users on req.UserId equals user.Id
                              select new JoinOrgRequestViewModel
                              {
                                  Id = req.Id,
                                  PersonName = user.Name,
                                  UserName = user.UserName,
                                  OrganizationName = orgName,
                                  CreatedAt = req.WhenCreated
                              };
        }

        public async Task OnGet(string anchor)
        {
            await InitPendingRequests(anchor);
        }

        public async Task<IActionResult> OnPostAccept(string anchor, int id, bool accepted)
        {
            await InitPendingRequests(anchor);
            var request = await joinOrganizationRequestStore.Requests
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }
            await joinOrganizationManager.Audit(request, User.DisplayName(), accepted);
            return RedirectToPage();
        }
    }

    public class JoinOrgRequestViewModel
    {
        public int Id { get; set; }

        public string PersonName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string OrganizationName { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }
    }
}
