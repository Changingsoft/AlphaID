using AlphaIdPlatform.Identity;
using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages;

public class SearchModel(IUserStore<NaturalPerson> store) : PageModel
{
    public IEnumerable<SearchedUser> Results { get; set; } = [];

    public IQueryableUserStore<NaturalPerson> Store { get; set; } = store as IQueryableUserStore<NaturalPerson> ??
                                                                    throw new NotSupportedException("不支持查询用户。");

    public IApplicationUserStore<NaturalPerson> ApplicationUserStore { get; set; } = store as IApplicationUserStore<NaturalPerson> ??
                                                                                 throw new NotSupportedException("不支持ApplicationStore。");

    public async Task<IActionResult> OnGetAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Page();

        if (MobilePhoneNumber.TryParse(q, out MobilePhoneNumber mobile))
        {
            NaturalPerson? person =
                await ApplicationUserStore.FindByPhoneNumberAsync(mobile.ToString(), HttpContext.RequestAborted);
            return person != null ? RedirectToPage("Detail/Index", new { id = person.Id }) : Page();
        }

        Results = from user in Store.Users
                  where user.SearchHint!.StartsWith(q) || user.Name!.StartsWith(q) || user.UserName!.StartsWith(q) || user.Email!.StartsWith(q)
                  select new SearchedUser()
                  {
                      Id = user.Id,
                      Name = user.Name,
                      UserName = user.UserName,
                      PhoneNumber = user.PhoneNumber,
                      PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                      Email = user.Email,
                      EmailConfirmed = user.EmailConfirmed,
                      Gender = user.Gender,
                      DateOfBirth = user.DateOfBirth,
                  };

        return Page();
    }

    public class SearchedUser
    {
        public string Id { get; set; } = null!;

        public string? Name { get; set; }

        public string? UserName { get; set; }

        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public Gender? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}