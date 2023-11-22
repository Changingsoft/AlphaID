

namespace IdSubjects.RealName;

/// <summary>
/// 实名信息管理器。
/// </summary>
public class RealNameManager
{
    private readonly IRealNameStateStore stateStore;
    private readonly NaturalPersonManager naturalPersonManager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stateStore"></param>
    /// <param name="naturalPersonManager"></param>
    public RealNameManager(IRealNameStateStore stateStore, NaturalPersonManager naturalPersonManager)
    {
        this.stateStore = stateStore;
        this.naturalPersonManager = naturalPersonManager;
    }


    /// <summary>
    /// 获取与自然人相关的实名状态信息。
    /// </summary>
    /// <param name="person"></param>
    /// <returns>与自然人相关的实名状态。如果没有，则返回null。</returns>
    public virtual async Task<RealNameState?> GetRealNameStateAsync(NaturalPerson person)
    {
        return await this.stateStore.FindByIdAsync(person.Id);
    }

    /// <summary>
    /// 向指定的自然人添加实名认证信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="validation"></param>
    /// <returns></returns>
    public async Task AddValidationAsync(NaturalPerson person, RealNameValidation validation)
    {
        var state = await this.GetRealNameStateAsync(person);
        if(state == null)
        {
            state = new RealNameState(person.Id)
            {
                
            };
            await this.stateStore.CreateAsync(state);
        }
        state.Validations.Add(validation);
        await this.stateStore.UpdateAsync(state);

        await this.naturalPersonManager.UpdateAsync(person);

    }

    internal async Task<IdOperationResult> UpdateAsync(RealNameState realNameState)
    {
        return await this.stateStore.UpdateAsync(realNameState);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> DeleteAsync(RealNameState state)
    {
        return await this.stateStore.DeleteAsync(state);
    }
}
