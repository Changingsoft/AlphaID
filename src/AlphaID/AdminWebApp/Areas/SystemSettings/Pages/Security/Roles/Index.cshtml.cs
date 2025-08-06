using AdminWebApp.Domain.Security;
using AlphaIdPlatform.Admin;
using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Roles
{
    public class IndexModel(UserInRoleManager userInRoleManager) : PageModel
    {
        public IEnumerable<RoleInfoModel> Roles { get; set; } = [];

        public void OnGet()
        {
            Roles = from role in RoleConstants.Roles
                    join userInRole in userInRoleManager.UserInRoles on role.Name equals userInRole.RoleName into userRoles
                    select new RoleInfoModel
                    {
                        Name = role.Name,
                        DisplayName = role.DisplayName,
                        Description = role.Description,
                        MemberCount = userRoles.Count()
                    };
        }

        public class RoleInfoModel
        {
            public string Name { get; set; } = null!;

            public string DisplayName { get; set; } = null!;

            public string? Description { get; set; }

            public int MemberCount { get; set; }
        }
    }
}
