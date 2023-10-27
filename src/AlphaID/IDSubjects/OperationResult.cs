namespace IDSubjects;

/// <summary>
/// 表示操作的结果。
/// </summary>
[Obsolete("应使用 IdentityResult。")]
public class OperationResult
{
    /// <summary>
    /// 初始化操作结果。
    /// </summary>
    internal OperationResult()
    {
        this.Errors = new List<string>();
    }

    /// <summary>
    /// 使用错误列表初始化操作结果。
    /// </summary>
    /// <param name="errors"></param>
    internal OperationResult(IEnumerable<string> errors)
    {
        this.Errors = new List<string>(errors);
    }

    /// <summary>
    /// 错误列表。
    /// </summary>
    public IList<string> Errors { get; set; }

    /// <summary>
    /// 指示操作是否成功。
    /// </summary>
    public bool IsSuccess { get { return !this.Errors.Any(); } }


    static OperationResult()
    {
        Success = new OperationResult();
    }

    /// <summary>
    /// Gets an OperationResult instance thant point to success.
    /// </summary>
    public static readonly OperationResult Success;


    /// <summary>
    /// Create an OperationResult with one Error.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static OperationResult Error(string v)
    {
        var result = new OperationResult(new string[] { v });
        return result;
    }
}