using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminWebApp.Areas.UserManagement.Pages
{
    public class LockedUsersModel(IUserStore<NaturalPerson> userStore, ApplicationUserManager<NaturalPerson> applicationUserManager) : PageModel
    {
        public IQueryableUserStore<NaturalPerson> UserStore { get; } = userStore as IQueryableUserStore<NaturalPerson> ?? throw new NotSupportedException("不支持查询");

        public IEnumerable<UserModel> Users { get; set; } = [];

        public void OnGet()
        {
            Users = from user in UserStore.Users
                    where user.LockoutEnd > DateTimeOffset.UtcNow
                    select new UserModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        LockoutEnd = user.LockoutEnd!.Value
                    };
        }

        public async Task<IActionResult> OnPostUnlockAsync(string id)
        {
            var user = await applicationUserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await applicationUserManager.UnlockUserAsync(user);
            return RedirectToPage();
        }

        public class UserModel
        {
            public string Id { get; set; } = null!;

            public string? UserName { get; set; }

            public string? Name { get; set; }

            public DateTimeOffset LockoutEnd { get; set; }
        }
    }
}
