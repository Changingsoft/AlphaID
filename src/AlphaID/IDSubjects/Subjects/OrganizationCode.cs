using System.Text.RegularExpressions;

namespace IDSubjects.Subjects;

/// <summary>
/// 表示组织机构代码。
/// </summary>
public struct OrganizationCode
{
    private string code;

    /// <summary>
    /// 使用组织机构代码（不含校验位）创建组织机构代码。
    /// </summary>
    /// <param name="Code"></param>
    public OrganizationCode(string Code)
    {
        if (string.IsNullOrWhiteSpace(Code))
            throw new ArgumentException("Code is empty or null.");
        var trimcode = Code.Trim().ToUpper();
        if (trimcode.Length != 8)
            throw new ArgumentException("Code length error.");
        this.code = trimcode;
        this.CheckCode = Check(trimcode);
    }

    /// <summary>
    /// 获取或设置组织机构代码本体。
    /// </summary>
    public string Code
    {
        get
        {
            return this.code;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Code is empty or null.");
            var trimcode = value.Trim().ToUpper();
            if (trimcode.Length != 8)
                throw new ArgumentException("Code length error.");
            this.code = trimcode;
            this.CheckCode = Check(trimcode);
        }
    }

    /// <summary>
    /// 获取校验码。
    /// </summary>
    public char CheckCode { get; private set; }

    /// <summary>
    /// 已重写，输出组织机构代码的可读形式。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.ToString(false);
    }

    /// <summary>
    /// 输出组织机构代码，如果指示为机读形式，则忽略中间的连字符。
    /// </summary>
    /// <param name="AsMachineFormat"></param>
    /// <returns></returns>
    public string ToString(bool AsMachineFormat)
    {
        return AsMachineFormat ? this.code + this.CheckCode.ToString() : this.code + "-" + this.CheckCode.ToString();
    }

    /// <summary>
    /// 已重写。获取组织机构代码的HashCode.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }

    /// <summary>
    /// 已重写，判定两个组织机构代码是否值相等。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return obj is OrganizationCode code1 ? this == code1 : base.Equals(obj);
    }

    /// <summary>
    /// 已重载。判定两个组织机构代码是否值相等。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(OrganizationCode a, OrganizationCode b)
    {
        return a.code == b.code && a.CheckCode == b.CheckCode;
    }

    /// <summary>
    /// 已重载。判定两个组织机构代码是否值不相等。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(OrganizationCode a, OrganizationCode b)
    {
        return !(a == b);
    }

    private static char Check(string Code)
    {
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            var charindex = charset.IndexOf(Code[i]);
            if (charindex < 0)
                throw new ArgumentException("无效字符");
            sum += charindex * weight[i];
        }
        return checkcodeCharset[(11 - (sum % 11)) % 11]; //处理当余数为0时，11-0 = 11，超出字符集范围，再次取模得0，约束在0-10范围内。
    }

    private const string checkcodeCharset = "0123456789X";
    private const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly int[] weight = new int[] { 3, 7, 9, 10, 5, 8, 4, 2 };
    private const string pattern = @"^([0-9A-Z]{8})-?([0-9X])$";

    /// <summary>
    /// 将给定的字符串文本匹配为组织机构代码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static OrganizationCode Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Input is null or empty");
        var trimedStr = s.Trim().ToUpper();

        var match = Regex.Match(trimedStr, pattern);
        if (!match.Success)
            throw new ArgumentException("Invalid input value format.");

        OrganizationCode newcode = new(match.Groups[1].Value);
        char inputCheckCode = char.Parse(match.Groups[2].Value);
        return inputCheckCode != newcode.CheckCode ? throw new ArgumentException("Invalid Checksum.") : newcode;
    }

    /// <summary>
    /// 尝试将给定文本匹配为组织机构代码。
    /// </summary>
    /// <param name="s"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParse(string s, out OrganizationCode result)
    {
        result = new OrganizationCode();
        if (string.IsNullOrWhiteSpace(s))
            return false;
        var trimedStr = s.Trim().ToUpper();

        var match = Regex.Match(trimedStr, pattern);
        if (!match.Success)
            return false;

        OrganizationCode newcode = new(match.Groups[1].Value);

        if (!char.TryParse(match.Groups[2].Value, out char inputCheckCode))
            return false;
        if (inputCheckCode != newcode.CheckCode)
            return false;

        result = newcode;
        return true;
    }

    /// <summary>
    /// 获取一个值，指示一个特定文本是否是组织机构代码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsOrganizationCode(string s)
    {
        return TryParse(s, out _);
    }
}
