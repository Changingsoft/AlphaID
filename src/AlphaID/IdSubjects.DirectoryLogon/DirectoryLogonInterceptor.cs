using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IdSubjects.DirectoryLogon;
internal class DirectoryLogonInterceptor : NaturalPersonInterceptor
{
    private readonly ILogger<DirectoryLogonInterceptor>? logger;

    public DirectoryLogonInterceptor(ILogger<DirectoryLogonInterceptor>? logger)
    {
        this.logger = logger;
    }

    public override Task<IdentityResult> PreUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        this.logger?.LogInformation("目录更新服务拦截到用户信息更新。");
        return base.PreUpdateAsync(personManager, person);
    }
}
