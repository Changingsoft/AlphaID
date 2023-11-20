using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdSubjects.RealName;
internal class RealNameInterceptor : NaturalPersonInterceptor
{
    private readonly ILogger<RealNameInterceptor>? logger;
    private readonly RealNameManager manager;

    public RealNameInterceptor(ILogger<RealNameInterceptor>? logger, RealNameManager manager)
    {
        this.logger = logger;
        this.manager = manager;
    }

    public override async Task<IdentityResult> PreUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        List<IdentityError> errors = new();
        var realNameState = await this.manager.GetRealNameStateAsync(person);
        if (realNameState == null)
        {
            this.logger?.LogDebug("没有找到{person}的实名记录，不执行操作。", person);
            return IdentityResult.Success;
        }

        if (realNameState.ActionIndicator == ActionIndicator.PendingUpdate)
        {
            this.logger?.LogDebug("RealNameState指示了正在{person}等待更改，拦截器将放行此次更改。", person);
            return IdentityResult.Success;
        }

        //检查是否有对PersonName等作出的更改。
        var origin = personManager.Users.Single(p => p.Id == person.Id);
        if (!origin.PersonName.Equals(person.PersonName))
        {
            this.logger?.LogDebug("自然人名称不相等。原始 {origin}，传入 {incoming}。", origin.PersonName, person.PersonName);
            errors.Add(new IdentityError() { Code = "CannotEditPersonName", Description = "Cannot change person name when realname passed." });
        }
        if (origin.DateOfBirth != person.DateOfBirth)
            errors.Add(new IdentityError() { Code = "CannotEditBirthDate", Description = "Cannot change birth date when realname passed." });
        if (origin.Gender != person.Gender)
            errors.Add(new IdentityError() { Code = "CannotEditGender", Description = "Cannot change gender when realname passed." });

        return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    public override async Task PostUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        var realNameState = await this.manager.GetRealNameStateAsync(person);
        if (realNameState == null)
        {
            this.logger?.LogDebug("没有找到{person}的实名记录，不执行操作。", person);
            return;
        }

        if (realNameState.ActionIndicator == ActionIndicator.PendingUpdate)
        {
            this.logger?.LogDebug("RealNameState指示了正在{person}等待更改，拦截器将状态改为已更改。", person);
            realNameState.ActionIndicator = ActionIndicator.Updated;
            await this.manager.UpdateAsync(realNameState);
        }
    }
}