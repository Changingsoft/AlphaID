namespace AdminWebApp.Areas.OpenIDConnect;

public class OidcOperationResult
{
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
    /// Gets an IdOperationResult instance that point to success.
    /// </summary>
    public static OidcOperationResult Success { get; } = new() { Succeeded = true };

    /// <summary>
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static OidcOperationResult Failed(params string[] errors)
    {
        var result = new OidcOperationResult { Succeeded = false };
        result._errors.AddRange(errors);
        return result;
    }
}