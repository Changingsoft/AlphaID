using DirectoryLogon;
using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail.Account;

public class BindDirectoryAccountModel : PageModel
{
    private readonly NaturalPersonManager personManager;
    private readonly LogonAccountManager logonAccountManager;
    private readonly DirectoryServiceManager directoryServiceManager;

    public BindDirectoryAccountModel(NaturalPersonManager personManager, LogonAccountManager logonAccountManager, DirectoryServiceManager directoryServiceManager)
    {
        this.personManager = personManager;
        this.logonAccountManager = logonAccountManager;
        this.directoryServiceManager = directoryServiceManager;
    }

    public IEnumerable<DirectoryService> DirectoryServices => this.directoryServiceManager.Services;

    public NaturalPerson Person { get; set; } = default!;

    public IEnumerable<DirectorySearchItem> SearchItems { get; set; } = Array.Empty<DirectorySearchItem>();

    public async Task<IActionResult> OnGet(string anchor)
    {
        var person = await this.personManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        return this.Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string anchor, int serviceId, string keywords)
    {
        var person = await this.personManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;

        var directoryService = await this.directoryServiceManager.FindByIdAsync(serviceId);
        if (directoryService == null)
            return this.Page();

        this.SearchItems = this.logonAccountManager.Search(directoryService, $"(anr={keywords})");
        return this.Page();
    }

    public async Task<IActionResult> OnPostBindAsync(string anchor, int serviceId, Guid entryGuid)
    {
        var person = await this.personManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;

        var directoryService = await this.directoryServiceManager.FindByIdAsync(serviceId);
        if (directoryService == null)
            return this.Page();

        try
        {
            await this.logonAccountManager.BindExistsAccount(directoryService, person, entryGuid);
            return this.RedirectToPage("DirectoryAccounts", new { anchor });
        }
        catch (Exception)
        {

            throw;
        }
    }
}
