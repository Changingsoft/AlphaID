namespace IdSubjects;

/// <summary>
/// 表示操作的结果。
/// </summary>
public class IdOperationResult
{
    /// <summary>
    /// 获取一个表示成功状态的IdOperationResult。
    /// </summary>
    public static readonly IdOperationResult Success = new() { Succeeded = true };

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
    public static IdOperationResult Failed(params string[] errors)
    {
        var result = new IdOperationResult { Succeeded = false };
        result._errors.AddRange(errors);
        return result;
    }
}