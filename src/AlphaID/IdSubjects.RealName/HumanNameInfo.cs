using Microsoft.EntityFrameworkCore;

namespace IdSubjects.RealName;

/// <summary>
///     表示个人名称。
/// </summary>
[Owned]
public record HumanNameInfo
{
    /// <summary>
    ///     For persistence.
    /// </summary>
    public HumanNameInfo()
    {
    }

    /// <summary>
    ///     Create a HumanNameInfo.
    /// </summary>
    /// <param name="fullName"></param>
    /// <param name="surname"></param>
    /// <param name="givenName"></param>
    /// <param name="middleName"></param>
    public HumanNameInfo(string fullName, string? surname = null, string? givenName = null, string? middleName = null)
    {
        Surname = surname;
        GivenName = givenName;
        FullName = fullName;
        MiddleName = middleName;
    }

    /// <summary>
    /// 姓氏，也表示为Family Name。
    /// </summary>
    public string? Surname { get; protected set; }

    /// <summary>
    /// 中间名。
    /// </summary>
    public string? MiddleName { get; protected set; }

    /// <summary>
    /// 名。
    /// </summary>
    public string? GivenName { get; protected set; }

    /// <summary>
    /// 完整人名。
    /// </summary>
    public string FullName { get; protected set; } = null!;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return FullName;
    }
}