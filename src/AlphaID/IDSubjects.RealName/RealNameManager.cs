

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
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
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

        validation.Document.ApplyRealName(person);
        state.Validations.Add(validation);
        state.ActionIndicator = ActionIndicator.PendingUpdate;
        await this.stateStore.UpdateAsync(state);

        await this.naturalPersonManager.UpdateAsync(person);

    }

    internal async Task<IdOperationResult> UpdateAsync(RealNameState realNameState)
    {
        return await this.stateStore.UpdateAsync(realNameState);
    }
}
