namespace ChineseName;

/// <summary>
/// Chinese Person Name.
/// </summary>
public class ChinesePersonName
{
    /// <summary>
    /// </summary>
    protected ChinesePersonName()
    {
    }

    /// <summary>
    /// 使用姓氏、名字和拼音初始化中国人名称。
    /// </summary>
    /// <param name="surname"></param>
    /// <param name="givenName"></param>
    /// <param name="phoneticSurname"></param>
    /// <param name="phoneticGivenName"></param>
    public ChinesePersonName(string? surname, string? givenName, string? phoneticSurname, string? phoneticGivenName)
    {
        Surname = surname;
        GivenName = givenName;
        PhoneticSurname = phoneticSurname;
        PhoneticGivenName = phoneticGivenName;
    }

    /// <summary>
    /// 姓氏。
    /// </summary>
    public string? Surname { get; protected set; }

    /// <summary>
    /// 名字（不含姓氏部分）。
    /// </summary>
    public string? GivenName { get; protected set; }

    /// <summary>
    /// 姓名。
    /// </summary>
    public string FullName => $"{Surname}{GivenName}";

    /// <summary>
    /// </summary>
    public string? PhoneticSurname { get; protected set; }

    /// <summary>
    /// </summary>
    public string? PhoneticGivenName { get; protected set; }

    /// <summary>
    /// </summary>
    public string PhoneticName => $"{PhoneticSurname}{PhoneticGivenName}".Trim();

    /// <summary>
    /// Override. Return full name of Person.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{FullName}|{PhoneticName}";
    }
}