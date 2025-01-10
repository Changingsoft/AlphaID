using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdSubjects.RealName;

internal class RealNameUpdateInterceptor(ILogger<RealNameUpdateInterceptor>? logger, IRealNameAuthenticationStore store)
    : NaturalPersonUpdateInterceptor
{
    private IEnumerable<RealNameAuthentication> _pendingAuthentications = [];

    public override async Task<IdentityResult> PreUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        List<IdentityError> errors = [];


        IQueryable<RealNameAuthentication> personAuthentications = store.FindByPerson(person);
        if (!personAuthentications.Any())
        {
            logger?.LogDebug("没有找到{person}的实名记录，不执行操作。", person);
            return IdentityResult.Success;
        }

        _pendingAuthentications = [.. personAuthentications.Where(a => !a.Applied)];
        if (_pendingAuthentications.Any())
        {
            foreach (RealNameAuthentication authentication in _pendingAuthentications)
            {
                authentication.ApplyToPerson(person);
                logger?.LogDebug("拦截器使用{authentication}覆盖了{person}的信息。", authentication, person);
            }

            return IdentityResult.Success;
        }

        //检查是否有对PersonName等作出的更改。
        NaturalPerson? origin = await personManager.GetOriginalAsync(person);
        if (origin != null)
        {
            if (!origin.PersonName.Equals(person.PersonName))
            {
                logger?.LogDebug("自然人名称不相等。原始 {origin}，传入 {incoming}。", origin.PersonName, person.PersonName);
                errors.Add(new IdentityError
                    { Code = "CannotEditPersonName", Description = "Cannot change person name when realname passed." });
            }

            if (origin.DateOfBirth != person.DateOfBirth)
                errors.Add(new IdentityError
                    { Code = "CannotEditBirthDate", Description = "Cannot change birth date when realname passed." });
            if (origin.Gender != person.Gender)
                errors.Add(new IdentityError
                    { Code = "CannotEditGender", Description = "Cannot change gender when realname passed." });
        }

        return errors.Count != 0 ? IdentityResult.Failed([.. errors]) : IdentityResult.Success;
    }

    public override async Task PostUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        foreach (RealNameAuthentication authentication in _pendingAuthentications)
        {
            authentication.Applied = true;
            await store.UpdateAsync(authentication);
            logger?.LogDebug("已将{authentication}的应用状态改为已更改。", authentication);
        }
    }
}