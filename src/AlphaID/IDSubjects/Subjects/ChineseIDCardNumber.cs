using System.Text.RegularExpressions;

namespace IDSubjects.Subjects;

/// <summary>
/// 表示一个中华人民共和国居民身份证号。
/// </summary>
public partial struct ChineseIDCardNumber
{
    private readonly int regionCode;
    private readonly DateTime dateOfBirth;
    private readonly int sequence;
    private readonly char checkcode;
    private readonly int version;

    /// <summary>
    /// 使用指定的版本、区划代码、生日和序列号创建一个身份证号码。
    /// </summary>
    /// <param name="Version"></param>
    /// <param name="RegionCode"></param>
    /// <param name="DateOfBirth"></param>
    /// <param name="Sequence"></param>
    public ChineseIDCardNumber(int Version, int RegionCode, DateTime DateOfBirth, int Sequence)
    {
        if (Version != ChineseIDCardNumberVersion.V1 && Version != ChineseIDCardNumberVersion.V2)
            throw new OverflowException("Version Overflow.");
        if (RegionCode is < 100000 or > 999999)
            throw new OverflowException("Region code overflow.");
        if (DateOfBirth > DateTime.UtcNow)
            throw new ArgumentException("Date of Birth in the future.");
        if (Sequence > 999)
            throw new OverflowException("Sequence out of range.");

        this.version = Version;
        this.regionCode = RegionCode;
        this.dateOfBirth = DateOfBirth.Date;
        this.sequence = Sequence;
        this.checkcode = CalculateCheckCode(string.Format(qulifiedFormat, this.regionCode, this.dateOfBirth, this.sequence));
    }

    /// <summary>
    /// 获取或设置该身份证号码的版本。
    /// </summary>
    public readonly int Version
    {
        get { return this.version; }
    }

    /// <summary>
    /// 获取或设置行政区划编码。
    /// </summary>
    public readonly int RegionCode
    {
        get { return this.regionCode; }
    }

    /// <summary>
    /// 获取或设置出生日期。
    /// </summary>
    public readonly DateTime DateOfBirth
    {
        get { return this.dateOfBirth; }
    }

    /// <summary>
    /// 获取或设置序列号。
    /// </summary>
    public readonly int Sequence
    {
        get { return this.sequence; }
    }

    /// <summary>
    /// 获取一个值，指示性别。
    /// </summary>
    public readonly Sex Sex
    {
        get
        {
            return this.sequence % 2 == 1 ? Sex.Male : Sex.Female;
        }
    }

    /// <summary>
    /// 获取一个值，表示身份证号码。
    /// </summary>
    public readonly string NumberString
    {
        get
        {
            return this.ToString();
        }
    }

    /// <summary>
    /// 已重写，输入身份证号码。
    /// </summary>
    /// <returns></returns>
    public override readonly string ToString()
    {

        return this.ToString(this.version);
    }

    /// <summary>
    /// 已重写。获取用于哈希表的对象哈希。
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }

    /// <summary>
    /// 使用指定的版本号获取对应格式的身份证号码。
    /// </summary>
    /// <param name="Version"></param>
    /// <returns></returns>
    public readonly string ToString(int Version)
    {
        return Version == ChineseIDCardNumberVersion.V1
            ? this.regionCode.ToString("000000") + this.dateOfBirth.ToString("yyMMdd") + this.sequence.ToString("000")
            : Version == ChineseIDCardNumberVersion.V2
            ? this.regionCode.ToString("000000") + this.dateOfBirth.ToString("yyyyMMdd") + this.sequence.ToString("000") + this.checkcode.ToString()
            : throw new NotSupportedException("Version Not supported.");
    }

    /// <summary>
    /// 已重载。比较两个身份证号码是相等性。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(ChineseIDCardNumber a, ChineseIDCardNumber b)
    {
        return a.version == b.version && a.regionCode == b.regionCode && a.dateOfBirth == b.dateOfBirth && a.sequence == b.sequence;
    }

    /// <summary>
    /// 已重载。比较两个身份证号码的不等性。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(ChineseIDCardNumber a, ChineseIDCardNumber b)
    {
        return !(a == b);
    }

    /// <summary>
    /// 已重载，比较两个身份证号码的相等性。该方法的行为与相等运算符重载的行为一致。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override readonly bool Equals(object? obj)
    {
        return obj != null && obj is ChineseIDCardNumber number && this == number;
    }

    /// <summary>
    /// 将输入的字符串匹配为身份证号码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static ChineseIDCardNumber Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Input is whole whitespace or null.");
        var dataStr = s.Trim().ToUpper();

        Match match;
        int ver;
        if (dataStr.Length == 18)
        {
            match = CardNumber18().Match(dataStr);
            ver = ChineseIDCardNumberVersion.V2;
        }
        else if (dataStr.Length == 15)
        {
            match = CardNumber15().Match(dataStr);
            ver = ChineseIDCardNumberVersion.V1;
        }
        else
            throw new FormatException("身份证格式错误。");

        DateTime dateOfBirth = ver == ChineseIDCardNumberVersion.V1
            ? new DateTime(int.Parse("19" + match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))
            : new DateTime(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
        ChineseIDCardNumber number = new(ver, int.Parse(match.Groups[1].Value), dateOfBirth, int.Parse(match.Groups[5].Value));
        return ver == ChineseIDCardNumberVersion.V2 && number.checkcode != dataStr[17] ? throw new ArgumentException("校验错误") : number;
    }

    /// <summary>
    /// 尝试将输入的字符串匹配为一个身份证号码。
    /// </summary>
    /// <param name="s">输入的字符串。</param>
    /// <param name="number">若匹配正确，则输出改身份证号码结构。否则为默认值。</param>
    /// <returns>若匹配成功，返回true，否则为false.</returns>
    public static bool TryParse(string s, out ChineseIDCardNumber number)
    {
        number = new ChineseIDCardNumber();
        if (string.IsNullOrWhiteSpace(s))
            return false;

        var dataStr = s.Trim().ToUpper();
        Match match;
        int ver;
        if (dataStr.Length == 18)
        {
            match = CardNumber18().Match(dataStr);
            ver = ChineseIDCardNumberVersion.V2;
        }
        else if (dataStr.Length == 15)
        {
            match = CardNumber15().Match(dataStr);
            ver = ChineseIDCardNumberVersion.V1;
        }
        else
            return false;


        if (!int.TryParse(ver == ChineseIDCardNumberVersion.V1 ? "19" + match.Groups[2].Value : match.Groups[2].Value, out int year))
            return false;
        if (!int.TryParse(match.Groups[3].Value, out int month))
            return false;
        if (!int.TryParse(match.Groups[4].Value, out int day))
            return false;

        DateTime dateOfBirth;
        try
        {
            dateOfBirth = new DateTime(year, month, day);
        }
        catch
        { return false; }

        if (dateOfBirth > DateTime.UtcNow) return false;

        number = new ChineseIDCardNumber(ver, int.Parse(match.Groups[1].Value), dateOfBirth, int.Parse(match.Groups[5].Value));
        return ver != ChineseIDCardNumberVersion.V2 || number.checkcode == dataStr[17];
    }

    /// <summary>
    /// 确定一个字符串是否为身份证号码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsChineseIDCardNumber(string s)
    {
        return TryParse(s, out _);
    }

    private static readonly int[] Weight = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
    private const string CheckCodeSet = "10X98765432";
    private const string pattern18 = @"^([1-9]\d{5})(\d{4})(\d{2})(\d{2})(\d{3})(\d|X)$";
    private const string pattern15 = @"^([1-9]\d{5})(\d{2})(\d{2})(\d{2})(\d{3})$";
    private const string qulifiedFormat = "{0:000000}{1:yyyyMMdd}{2:000}";

    private static char CalculateCheckCode(string data)
    {
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            sum += int.Parse(data[i].ToString()) * Weight[i];
        }
        return CheckCodeSet[sum % 11];
    }

    [GeneratedRegex("^([1-9]\\d{5})(\\d{4})(\\d{2})(\\d{2})(\\d{3})(\\d|X)$")]
    private static partial Regex CardNumber18();
    [GeneratedRegex("^([1-9]\\d{5})(\\d{2})(\\d{2})(\\d{2})(\\d{3})$")]
    private static partial Regex CardNumber15();
}