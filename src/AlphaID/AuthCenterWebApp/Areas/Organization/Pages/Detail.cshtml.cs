using AlphaID.EntityFramework;
using AlphaIDPlatform.Security;
using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages;

public class DetailModel : PageModel
{
    private readonly IDSubjectsDbContext dbContext;

    public DetailModel(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    public OrganizationMember Data { get; set; } = default!;

    public IActionResult OnGet()
    {
        var userId = this.User.SubjectId();
        if (userId == null)
        {
            return this.NotFound();
        }
        var data = this.dbContext.OrganizationMembers.FirstOrDefault(p => p.OrganizationId == this.Id && p.PersonId == userId);
        if (data == null)
        {
            return this.NotFound();
        }
        this.Data = data;
        return this.Page();
    }
}
