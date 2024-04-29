using System.Text.RegularExpressions;

namespace IdSubjects.Subjects;

/// <summary>
///     表示一个中华人民共和国居民身份证号。
/// </summary>
public readonly struct ChineseIdCardNumber
{
    private readonly DateOnly _dateOfBirth;
    private readonly char _checkCode;

    /// <summary>
    ///     使用指定的版本、区划代码、生日和序列号创建一个身份证号码。
    /// </summary>
    /// <param name="version"></param>
    /// <param name="areaNumber"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="sequence"></param>
    public ChineseIdCardNumber(int version, string areaNumber, DateOnly dateOfBirth, int sequence)
    {
        if (version != V1 && version != V2)
            throw new OverflowException("Version Overflow.");
        ArgumentNullException.ThrowIfNull(areaNumber);
        if (!s_areaNumberRegex.IsMatch(areaNumber))
            throw new ArgumentException(Resources.Invalid_region_code_, nameof(areaNumber));
        if (sequence is < 0 or > 999)
            throw new OverflowException("Sequence out of range.");

        Version = version;
        AreaNumber = areaNumber;
        _dateOfBirth = dateOfBirth;
        Sequence = sequence;
        _checkCode = CalculateCheckCode(string.Format(QualifiedFormat, AreaNumber, _dateOfBirth, Sequence));
    }

    /// <summary>
    ///     使用行政区划代码、出生日期和序列号创建一个第二代身份证号码。
    /// </summary>
    /// <param name="areaNumber"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="sequence"></param>
    public ChineseIdCardNumber(string areaNumber, DateOnly dateOfBirth, int sequence)
        : this(V2, areaNumber, dateOfBirth, sequence)
    {
    }

    /// <summary>
    ///     获取该身份证号码的版本。
    /// </summary>
    public int Version { get; }

    /// <summary>
    ///     获取行政区划编码。
    /// </summary>
    public string AreaNumber { get; }

    /// <summary>
    ///     获取出生日期。
    /// </summary>
    public DateOnly DateOfBirth => _dateOfBirth;

    /// <summary>
    ///     获取序列号。
    /// </summary>
    public int Sequence { get; }

    /// <summary>
    ///     获取一个值，指示性别。
    /// </summary>
    public Gender Gender => Sequence % 2 == 1 ? Gender.Male : Gender.Female;

    /// <summary>
    ///     获取一个值，表示身份证号码。
    /// </summary>
    public string NumberString => ToString();

    /// <summary>
    ///     输出规范化身份证号码。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return ToString(Version);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    ///     使用指定的版本号获取对应格式的身份证号码。
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public string ToString(int version)
    {
        return version == V1
            ? AreaNumber + _dateOfBirth.ToString("yyMMdd") + Sequence.ToString("000")
            : version == V2
                ? AreaNumber + _dateOfBirth.ToString("yyyyMMdd") + Sequence.ToString("000") + _checkCode
                : throw new NotSupportedException("Version Not supported.");
    }

    /// <summary>
    ///     已重载。比较两个身份证号码是相等性。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(ChineseIdCardNumber a, ChineseIdCardNumber b)
    {
        return a.Version == b.Version && a.AreaNumber == b.AreaNumber && a._dateOfBirth == b._dateOfBirth &&
               a.Sequence == b.Sequence;
    }

    /// <summary>
    ///     已重载。比较两个身份证号码的不等性。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(ChineseIdCardNumber a, ChineseIdCardNumber b)
    {
        return !(a == b);
    }

    /// <summary>
    ///     已重载，比较两个身份证号码的相等性。该方法的行为与相等运算符重载的行为一致。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return obj is ChineseIdCardNumber number && this == number;
    }

    /// <summary>
    ///     将输入的字符串匹配为身份证号码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static ChineseIdCardNumber Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Input is whole whitespace or null.");
        string dataStr = s.Trim().ToUpper();

        Match match;
        int ver;
        if (dataStr.Length == 18)
        {
            match = s_p18.Match(dataStr);
            ver = V2;
        }
        else if (dataStr.Length == 15)
        {
            match = s_p15.Match(dataStr);
            ver = V1;
        }
        else
        {
            throw new FormatException("身份证格式错误。");
        }

        DateOnly dateOfBirth = ver == V1
            ? new DateOnly(int.Parse("19" + match.Groups[2].Value), int.Parse(match.Groups[3].Value),
                int.Parse(match.Groups[4].Value))
            : new DateOnly(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value),
                int.Parse(match.Groups[4].Value));
        ChineseIdCardNumber number = new(ver, match.Groups[1].Value, dateOfBirth, int.Parse(match.Groups[5].Value));
        return ver == V2 && number._checkCode != dataStr[17] ? throw new ArgumentException("校验错误") : number;
    }

    /// <summary>
    ///     尝试将输入的字符串匹配为一个身份证号码。
    /// </summary>
    /// <param name="s">输入的字符串。</param>
    /// <param name="number">若匹配正确，则输出改身份证号码结构。否则为默认值。</param>
    /// <returns>若匹配成功，返回true，否则为false.</returns>
    public static bool TryParse(string s, out ChineseIdCardNumber number)
    {
        number = new ChineseIdCardNumber();
        if (string.IsNullOrWhiteSpace(s))
            return false;

        string dataStr = s.Trim().ToUpper();
        Match match;
        int ver;
        if (dataStr.Length == 18)
        {
            match = s_p18.Match(dataStr);
            ver = V2;
        }
        else if (dataStr.Length == 15)
        {
            match = s_p15.Match(dataStr);
            ver = V1;
        }
        else
        {
            return false;
        }


        if (!int.TryParse(ver == V1 ? "19" + match.Groups[2].Value : match.Groups[2].Value, out int year))
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
        {
            return false;
        }

        if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now)) return false;

        number = new ChineseIdCardNumber(ver, match.Groups[1].Value, dateOfBirth, int.Parse(match.Groups[5].Value));
        return ver != V2 || number._checkCode == dataStr[17];
    }

    /// <summary>
    ///     确定一个字符串是否为身份证号码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsChineseIdCardNumber(string s)
    {
        return TryParse(s, out _);
    }

    private static readonly int[] s_weight = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1];
    private const string CheckCodeSet = "10X98765432";
    private const string QualifiedFormat = "{0}{1:yyyyMMdd}{2:000}";

    private static char CalculateCheckCode(string data)
    {
        var sum = 0;
        for (var i = 0; i < 17; i++) sum += int.Parse(data[i].ToString()) * s_weight[i];
        return CheckCodeSet[sum % 11];
    }

    private const string Pattern18 = @"^([1-9]\d{5})(\d{4})(\d{2})(\d{2})(\d{3})(\d|X)$";
    private const string Pattern15 = @"^([1-9]\d{5})(\\d{2})(\\d{2})(\\d{2})(\\d{3})$";
    private const string AreaNumberPattern = @"^([1-9]\d{5})$";
    private static readonly Regex s_p18 = new(Pattern18);
    private static readonly Regex s_p15 = new(Pattern15);
    private static readonly Regex s_areaNumberRegex = new(AreaNumberPattern);

    /// <summary>
    ///     表示第一代身份证号码。
    /// </summary>
    public static readonly int V1 = 1;

    /// <summary>
    ///     表示第二代身份证号码。
    /// </summary>
    public static readonly int V2 = 2;
}