using IDSubjects;

namespace AdminWebApp.Areas.OpenIDConnect;

public class OidcOperationResult
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


    static readonly OidcOperationResult success = new() { Succeeded = true };
    /// <summary>
    /// Gets an IdOperationResult instance thant point to success.
    /// </summary>
    public static OidcOperationResult Success => success;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static OidcOperationResult Failed(params string[] errors)
    {
        var result = new OidcOperationResult() { Succeeded = false };
        if (errors != null)
            result.errors.AddRange(errors);
        return result;
    }

}
