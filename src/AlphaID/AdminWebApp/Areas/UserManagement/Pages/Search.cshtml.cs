using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages;

public class SearchModel(IUserStore<NaturalPerson> store) : PageModel
{
    public IEnumerable<SearchedUser> Results { get; set; } = [];

    public IQueryableUserStore<NaturalPerson> Store { get; set; } = store as IQueryableUserStore<NaturalPerson> ??
                                                                    throw new NotSupportedException("不支持查询用户。");

    public IActionResult OnGet(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Page();

        if (MobilePhoneNumber.TryParse(q, out MobilePhoneNumber mobile))
        {
            string? personId = (from user in Store.Users
                                where user.PhoneNumber == mobile.ToString()
                                select user.Id).FirstOrDefault();
            return personId != null ? RedirectToPage("Detail/Index", new { anchor = personId }) : Page();
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