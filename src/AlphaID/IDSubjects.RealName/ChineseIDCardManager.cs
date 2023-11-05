using System.Transactions;

namespace IDSubjects.RealName;

/// <summary>
/// 中国居民身份证管理器。
/// </summary>
[Obsolete]
public class ChineseIDCardManager
{
    private readonly IChineseIDCardValidationStore store;

    /// <summary>
    /// 初始化实名验证器。
    /// </summary>
    /// <param name="store"></param>
    public ChineseIDCardManager(IChineseIDCardValidationStore store)
    {
        this.store = store;
    }

    /// <summary>
    /// 获取等待审批的实名认证请求。
    /// </summary>
    public IEnumerable<ChineseIDCardValidation> PendingValidations
    {
        get
        {
            return this.store.RealNameValidations.Where(p => p.Result == null).OrderBy(p => p.CommitTime);
        }
    }

    /// <summary>
    /// 获取Validations.
    /// </summary>
    public IQueryable<ChineseIDCardValidation> Validations => this.store.RealNameValidations;

    /// <summary>
    /// 获取当前待定的实名认证。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public Task<ChineseIDCardValidation?> GetPendingRequestAsync(NaturalPerson person)
    {
        return this.store.GetPendingRequestAsync(person);
    }

    /// <summary>
    /// 获取当前已通过的实名认证。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public Task<ChineseIDCardValidation?> GetCurrentAsync(NaturalPerson person)
    {
        return this.store.GetCurrentAsync(person);
    }

    /// <summary>
    /// 更新实名认证信息。
    /// </summary>
    /// <param name="validation"></param>
    /// <returns></returns>
    public Task UpdateAsync(ChineseIDCardValidation validation)
    {
        return this.store.UpdateAsync(validation);
    }

    /// <summary>
    /// 执行验证，并根据验证结果，更新自然人信息。
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="validator"></param>
    /// <param name="accepted"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task ValidateAsync(ChineseIDCardValidation validation, string validator, bool accepted)
    {
        if (validation.Result != null)
        {
            throw new InvalidOperationException("已审核的实名认证不能再进行审核");
        }
        if (validation.ChineseIDCard == null)
        {
            throw new InvalidOperationException("未填写身份证信息时，不能启动验证");
        }
        if (validation.ChinesePersonName == null)
        {
            throw new InvalidOperationException("未填写姓名时，不能启动验证。");
        }

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        validation.Result = new ValidationResult(validator, DateTime.UtcNow, accepted);

        await this.store.UpdateAsync(validation);

        trans.Complete();
    }

    /// <summary>
    /// 提交实名认证请求。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="validation"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task CommitAsync(NaturalPerson person, ChineseIDCardValidation validation)
    {
        validation.Person = person;
        validation.PersonId = person.Id;

        validation.CommitTime = DateTime.UtcNow;
        await this.store.CreateAsync(validation);
    }

    /// <summary>
    /// Find a validation by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ValueTask<ChineseIDCardValidation?> FindByIdAsync(int id)
    {
        return this.store.FindByIdAsync(id);
    }
}
