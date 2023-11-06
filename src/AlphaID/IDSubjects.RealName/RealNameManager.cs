namespace IDSubjects.RealName;

/// <summary>
/// 实名信息管理器。
/// </summary>
public class RealNameManager
{
    private readonly IRealNameStore store;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    public RealNameManager(IRealNameStore store)
    {
        this.store = store;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public RealNameInfo? GetRealNameInfo(NaturalPerson person)
    {
        return this.store.FindByPersonId(person.Id);
    }
}
