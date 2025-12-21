namespace Organizational;

/// <summary>
///    表示组织操作结果。
/// </summary>
public class OrganizationOperationResult
{
    /// <summary>
    /// 获取一个表示成功状态的IdOperationResult。
    /// </summary>
    public static readonly OrganizationOperationResult Success = new() { Succeeded = true };

    private readonly List<string> _errors = [];

    /// <summary>
    /// 错误列表。
    /// </summary>
    public IEnumerable<string> Errors => _errors;

    /// <summary>
    /// 指示操作是否成功。
    /// </summary>
    public bool Succeeded { get; protected set; }

    /// <summary>
    /// 使用若干错误消息创建一个失败状态的IdOperationResult。
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static OrganizationOperationResult Failed(params string[] errors)
    {
        var result = new OrganizationOperationResult { Succeeded = false };
        result._errors.AddRange(errors);
        return result;
    }
}
