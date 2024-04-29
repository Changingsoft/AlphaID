using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace IdSubjects.DirectoryLogon;

internal class DirectoryAccountCreateInterceptor(
    DirectoryServiceManager serviceManager,
    DirectoryAccountManager directoryAccountManager) : NaturalPersonCreateInterceptor
{
    private IEnumerable<DirectoryServiceDescriptor> DirectoryServices { get; set; } = [];

    public override Task<IdentityResult> PreCreateAsync(NaturalPersonManager personManager,
        NaturalPerson person,
        string? password = null)
    {
        DirectoryServices = serviceManager.Services.Where(s => s.AutoCreateAccount);
        foreach (DirectoryServiceDescriptor directoryService in DirectoryServices)
        {
            // 执行创建前检查
        }

        return base.PreCreateAsync(personManager, person, password);
    }

    public override async Task PostCreateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        foreach (DirectoryServiceDescriptor directoryService in DirectoryServices)
        {
            DirectoryAccount account = new(directoryService, person.Id);
            await directoryAccountManager.CreateAsync(personManager, account);
        }
    }
}