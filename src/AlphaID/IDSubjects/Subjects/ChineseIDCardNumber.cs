using System.Text.RegularExpressions;

namespace IdSubjects.Subjects;

/// <summary>
/// 表示一个中华人民共和国居民身份证号。
/// </summary>
public readonly partial struct ChineseIdCardNumber
{
    private readonly DateOnly dateOfBirth;
    private readonly char checkCode;

    /// <summary>
    /// 使用指定的版本、区划代码、生日和序列号创建一个身份证号码。
    /// </summary>
    /// <param name="version"></param>
    /// <param name="regionCode"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="sequence"></param>
    public ChineseIdCardNumber(int version, int regionCode, DateOnly dateOfBirth, int sequence)
    {
        if (version != ChineseIdCardNumberVersion.V1 && version != ChineseIdCardNumberVersion.V2)
            throw new OverflowException("Version Overflow.");
        if (regionCode is < 100000 or > 999999)
            throw new OverflowException("Region code overflow.");
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("Date of Birth in the future.");
        if (sequence > 999)
            throw new OverflowException("Sequence out of range.");

        this.Version = version;
        this.RegionCode = regionCode;
        this.dateOfBirth = dateOfBirth;
        this.Sequence = sequence;
        this.checkCode = CalculateCheckCode(string.Format(QualifiedFormat, this.RegionCode, this.dateOfBirth, this.Sequence));
    }

    /// <summary>
    /// 获取或设置该身份证号码的版本。
    /// </summary>
    public int Version { get; }

    /// <summary>
    /// 获取或设置行政区划编码。
    /// </summary>
    public int RegionCode { get; }

    /// <summary>
    /// 获取或设置出生日期。
    /// </summary>
    public DateOnly DateOfBirth
    {
        get { return this.dateOfBirth; }
    }

    /// <summary>
    /// 获取或设置序列号。
    /// </summary>
    public int Sequence { get; }

    /// <summary>
    /// 获取一个值，指示性别。
    /// </summary>
    public Gender Gender
    {
        get
        {
            return this.Sequence % 2 == 1 ? Gender.Male : Gender.Female;
        }
    }

    /// <summary>
    /// 获取一个值，表示身份证号码。
    /// </summary>
    public string NumberString
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
    public override string ToString()
    {

        return this.ToString(this.Version);
    }

    /// <summary>
    /// 已重写。获取用于哈希表的对象哈希。
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }

    /// <summary>
    /// 使用指定的版本号获取对应格式的身份证号码。
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public string ToString(int version)
    {
        return version == ChineseIdCardNumberVersion.V1
            ? this.RegionCode.ToString("000000") + this.dateOfBirth.ToString("yyMMdd") + this.Sequence.ToString("000")
            : version == ChineseIdCardNumberVersion.V2
            ? this.RegionCode.ToString("000000") + this.dateOfBirth.ToString("yyyyMMdd") + this.Sequence.ToString("000") + this.checkCode.ToString()
            : throw new NotSupportedException("Version Not supported.");
    }

    /// <summary>
    /// 已重载。比较两个身份证号码是相等性。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(ChineseIdCardNumber a, ChineseIdCardNumber b)
    {
        return a.Version == b.Version && a.RegionCode == b.RegionCode && a.dateOfBirth == b.dateOfBirth && a.Sequence == b.Sequence;
    }

    /// <summary>
    /// 已重载。比较两个身份证号码的不等性。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(ChineseIdCardNumber a, ChineseIdCardNumber b)
    {
        return !(a == b);
    }

    /// <summary>
    /// 已重载，比较两个身份证号码的相等性。该方法的行为与相等运算符重载的行为一致。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return obj != null && obj is ChineseIdCardNumber number && this == number;
    }

    /// <summary>
    /// 将输入的字符串匹配为身份证号码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static ChineseIdCardNumber Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Input is whole whitespace or null.");
        var dataStr = s.Trim().ToUpper();

        Match match;
        int ver;
        if (dataStr.Length == 18)
        {
            match = p18.Match(dataStr);
            ver = ChineseIdCardNumberVersion.V2;
        }
        else if (dataStr.Length == 15)
        {
            match = p15.Match(dataStr);
            ver = ChineseIdCardNumberVersion.V1;
        }
        else
            throw new FormatException("身份证格式错误。");

        DateOnly dateOfBirth = ver == ChineseIdCardNumberVersion.V1
            ? new DateOnly(int.Parse("19" + match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))
            : new DateOnly(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
        ChineseIdCardNumber number = new(ver, int.Parse(match.Groups[1].Value), dateOfBirth, int.Parse(match.Groups[5].Value));
        return ver == ChineseIdCardNumberVersion.V2 && number.checkCode != dataStr[17] ? throw new ArgumentException("校验错误") : number;
    }

    /// <summary>
    /// 尝试将输入的字符串匹配为一个身份证号码。
    /// </summary>
    /// <param name="s">输入的字符串。</param>
    /// <param name="number">若匹配正确，则输出改身份证号码结构。否则为默认值。</param>
    /// <returns>若匹配成功，返回true，否则为false.</returns>
    public static bool TryParse(string s, out ChineseIdCardNumber number)
    {
        number = new ChineseIdCardNumber();
        if (string.IsNullOrWhiteSpace(s))
            return false;

        var dataStr = s.Trim().ToUpper();
        Match match;
        int ver;
        if (dataStr.Length == 18)
        {
            match = p18.Match(dataStr);
            ver = ChineseIdCardNumberVersion.V2;
        }
        else if (dataStr.Length == 15)
        {
            match = p15.Match(dataStr);
            ver = ChineseIdCardNumberVersion.V1;
        }
        else
            return false;


        if (!int.TryParse(ver == ChineseIdCardNumberVersion.V1 ? "19" + match.Groups[2].Value : match.Groups[2].Value, out int year))
            return false;
        if (!int.TryParse(match.Groups[3].Value, out int month))
            return false;
        if (!int.TryParse(match.Groups[4].Value, out int day))
            return false;

        DateOnly dateOfBirth;
        try
        {
            dateOfBirth = new DateOnly(year, month, day);
        }
        catch
        { return false; }

        if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now)) return false;

        number = new ChineseIdCardNumber(ver, int.Parse(match.Groups[1].Value), dateOfBirth, int.Parse(match.Groups[5].Value));
        return ver != ChineseIdCardNumberVersion.V2 || number.checkCode == dataStr[17];
    }

    /// <summary>
    /// 确定一个字符串是否为身份证号码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsChineseIdCardNumber(string s)
    {
        return TryParse(s, out _);
    }

    private static readonly int[] Weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
    private const string CheckCodeSet = "10X98765432";
    private const string QualifiedFormat = "{0:000000}{1:yyyyMMdd}{2:000}";

    private static char CalculateCheckCode(string data)
    {
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            sum += int.Parse(data[i].ToString()) * Weight[i];
        }
        return CheckCodeSet[sum % 11];
    }
#if NET8_0
    [GeneratedRegex("^([1-9]\\d{5})(\\d{4})(\\d{2})(\\d{2})(\\d{3})(\\d|X)$")]
    private static partial Regex CardNumber18();
    [GeneratedRegex("^([1-9]\\d{5})(\\d{2})(\\d{2})(\\d{2})(\\d{3})$")]
    private static partial Regex CardNumber15();
#endif

    private const string Pattern18 = @"^([1-9]\d{5})(\d{4})(\d{2})(\d{2})(\d{3})(\d|X)$";
    private const string Pattern15 = @"^([1-9]\\d{5})(\\d{2})(\\d{2})(\\d{2})(\\d{3})$";
    private static readonly Regex p18 = new(Pattern18);
    private static readonly Regex p15 = new(Pattern15);
}