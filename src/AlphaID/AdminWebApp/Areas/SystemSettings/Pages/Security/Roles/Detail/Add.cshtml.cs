using AdminWebApp.Domain.Security;
using AlphaIdPlatform.Admin;
using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Roles.Detail
{
    public class AddModel(UserInRoleManager manager, IQueryableUserStore<NaturalPerson> userStore) : PageModel
    {
        public Role Role { get; set; } = null!;

        [BindProperty]
        public string UserId { get; set; } = null!;

        public IActionResult OnGet(string name)
        {
            Role? role = RoleConstants.Roles.FirstOrDefault(r => r.Name == name);
            if (role == null)
            {
                return NotFound();
            }
            Role = role;
            return Page();
        }

        public async Task<IActionResult> OnPost(string name)
        {
            Role? role = RoleConstants.Roles.FirstOrDefault(r => r.Name == name);
            if (role == null)
            {
                return NotFound();
            }
            Role = role;

            if(!userStore.Users.Any(u => u.Id == UserId))
            {
                ModelState.AddModelError(nameof(UserId), "User not found.");
            }

            if (!ModelState.IsValid)
                return Page();

            await manager.AddRole(UserId, name);
            return RedirectToPage("Index", new { name });
        }
    }
}
