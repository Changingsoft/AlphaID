using AdminWebApp.Domain.Security;
using AlphaIdPlatform.Admin;
using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Roles.Detail
{
    public class IndexModel(UserInRoleManager manager, IQueryableUserStore<NaturalPerson> userStore) : PageModel
    {
        public Role Role { get; set; } = null!;

        public IEnumerable<UserInRoleModel> UserInrRoles { get; set; } = [];

        [TempData]
        public string? ResultMessage { get; set; }

        public IActionResult OnGet(string name)
        {
            Role? role = RoleConstants.Roles.FirstOrDefault(r => r.Name == name);
            if (role == null)
                return NotFound();
            Role = role;

            var userInRoles = manager.GetUserInRoles(name);

            UserInrRoles = from userInRole in userInRoles
                           join user in userStore.Users on userInRole.UserId equals user.Id
                           select new UserInRoleModel
                           {
                               RoleName = userInRole.RoleName,
                               UserId = user.Id,
                               UserName = user.UserName!,
                               DisplayName = user.Name!,
                               Email = user.Email,
                               PhoneNumber = user.PhoneNumber
                           };

            return Page();
        }

        public async Task<IActionResult> OnPostRemove(string name, string userId)
        {
            await manager.RemoveRole(userId, name);
            ResultMessage = "User removed from role successfully.";
            return RedirectToPage(new { name });
        }

        public class UserInRoleModel
        {
            public string RoleName { get; set; } = null!;
            public string UserId { get; set; } = null!;

            public string UserName { get; set; } = null!;

            public string DisplayName { get; set; } = null!;

            public string? Email { get; set; }

            public string? PhoneNumber { get; set; }
        }
    }
}
