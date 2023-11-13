namespace IDSubjects.ChineseName;

/// <summary>
/// 
/// </summary>
public static class CaseOptionsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caseOptions"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string? Normalize(this CaseOptions caseOptions, string? value)
    {
        if (value == null)
            return null;
        if (value == string.Empty)
            return string.Empty;

        value = value.ToUpper();

        return caseOptions switch
        {
            CaseOptions.Upper => value,
            CaseOptions.Lower => value.ToLower(),
            CaseOptions.FirstLetter => value[..1] + value[1..].ToLower(),
            _ => value,
        };
    }
}
