using AdminWebApp.Domain.Security;
using AlphaIdPlatform.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Roles.Detail
{
    public class AddModel(UserInRoleManager manager) : PageModel
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

            if (!ModelState.IsValid)
                return Page();

            await manager.AddRole(UserId, name);
            return RedirectToPage("Index", new { name });
        }
    }
}
