
namespace IdSubjects.RealName;

/// <summary>
/// 实名信息管理器。
/// </summary>
public class RealNameManager
{
    private readonly IRealNameStateStore stateStore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stateStore"></param>
    public RealNameManager(IRealNameStateStore stateStore)
    {
        this.stateStore = stateStore;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual async Task<RealNameState?> GetRealNameStateAsync(NaturalPerson person)
    {
        return await this.stateStore.FindByIdAsync(person.Id);
    }

    internal async Task<IdOperationResult> UpdateAsync(RealNameState realNameState)
    {
        return await this.stateStore.UpdateAsync(realNameState);
    }
}
