namespace IDSubjects;

/// <summary>
/// 表示操作的结果。
/// </summary>
public class IdOperationResult
{
    private readonly List<string> errors = new();

    /// <summary>
    /// 错误列表。
    /// </summary>
    public IEnumerable<string> Errors => this.errors;

    /// <summary>
    /// 指示操作是否成功。
    /// </summary>
    public bool Succeeded { get; protected set; }


    /// <summary>
    /// Gets an IdOperationResult instance thant point to success.
    /// </summary>
    public static readonly IdOperationResult Success = new() { Succeeded = true };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static IdOperationResult Failed(params string[] errors)
    {
        var result = new IdOperationResult() { Succeeded = false };
        result.errors.AddRange(errors);
        return result;
    }
}