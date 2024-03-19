using IdSubjects.Diagnostics;

namespace IdSubjects.RealName;
internal class RealNameDeleteInterceptor(IRealNameAuthenticationStore store) : NaturalPersonDeleteInterceptor
{
    public override async Task PostDeleteAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        await store.DeleteByPersonIdAsync(person.Id);
    }

}
