using AlphaIdPlatform.Identity;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class BindDirectoryAccountModel(
    UserManager<NaturalPerson> userManager,
    DirectoryAccountManager<NaturalPerson> directoryAccountManager,
    DirectoryServiceManager directoryServiceManager) : PageModel
{
    public IEnumerable<DirectoryService> DirectoryServices => directoryServiceManager.Services;

    public NaturalPerson Person { get; set; } = null!;

    public IEnumerable<DirectorySearchItem> SearchItems { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string anchor, int serviceId, string keywords)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;

        DirectoryService? directoryService = await directoryServiceManager.FindByIdAsync(serviceId);
        if (directoryService == null)
            return Page();

        SearchItems = directoryServiceManager.Search(directoryService, $"(anr={keywords})");
        return Page();
    }

    public async Task<IActionResult> OnPostBindAsync(string anchor, int serviceId, Guid entryGuid)
    {
        NaturalPerson? user = await userManager.FindByIdAsync(anchor);
        if (user == null)
            return NotFound();
        Person = user;

        DirectoryService? directoryService = await directoryServiceManager.FindByIdAsync(serviceId);
        if (directoryService == null)
            return Page();
        var logonAccount = new DirectoryAccount(directoryService, user.Id);
        await directoryAccountManager.LinkExistsAccount(user, directoryService, entryGuid.ToString());
        return RedirectToPage("DirectoryAccounts", new { anchor });
    }
}