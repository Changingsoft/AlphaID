using IdSubjects.Diagnostics;
using Microsoft.Extensions.Logging;

namespace IdSubjects.RealName;
internal class RealNameInterceptor : NaturalPersonManagerInterceptor
{
    private readonly ILogger<RealNameInterceptor>? logger;

    public RealNameInterceptor(ILogger<RealNameInterceptor>? logger)
    {
        this.logger = logger;
    }

    public override Task PreUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        this.logger?.LogInformation("即将开始更新个人信息。");
        return base.PreUpdateAsync(personManager, person);
    }
}
