using AdminWebApp.Domain.Security;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Roles
{
    public class IndexModel : PageModel
    {
        public IEnumerable<Role> Roles { get; set; } = [];

        public void OnGet()
        {
            Roles = RoleConstants.Roles;
        }
    }
}
