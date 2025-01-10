using IdSubjects;
using IdSubjects.DirectoryLogon;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class BindDirectoryAccountModel(
    NaturalPersonManager personManager,
    DirectoryAccountManager directoryAccountManager,
    DirectoryServiceManager directoryServiceManager) : PageModel
{
    public IEnumerable<DirectoryServiceDescriptor> DirectoryServices => directoryServiceManager.Services;

    public NaturalPerson Person { get; set; } = null!;

    public IEnumerable<DirectorySearchItem> SearchItems { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string anchor, int serviceId, string keywords)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;

        DirectoryServiceDescriptor? directoryService = await directoryServiceManager.FindByIdAsync(serviceId);
        if (directoryService == null)
            return Page();

        SearchItems = directoryAccountManager.Search(directoryService, $"(anr={keywords})");
        return Page();
    }

    public async Task<IActionResult> OnPostBindAsync(string anchor, int serviceId, Guid entryGuid)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;

        DirectoryServiceDescriptor? directoryService = await directoryServiceManager.FindByIdAsync(serviceId);
        if (directoryService == null)
            return Page();
        var logonAccount = new DirectoryAccount(directoryService, person.Id);
        await directoryAccountManager.BindExistsAccount(personManager, logonAccount, entryGuid.ToString());
        return RedirectToPage("DirectoryAccounts", new { anchor });
    }
}