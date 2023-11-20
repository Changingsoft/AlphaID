using System.Text.RegularExpressions;

namespace IdSubjects.Subjects;

/// <summary>
/// 移动电话号码。
/// </summary>
public struct MobilePhoneNumber
{
    /// <summary>
    /// 使用国家代码和移动电话号码初始化。
    /// </summary>
    /// <param name="countryCode">国家代码。</param>
    /// <param name="phoneNumber">电话号码。</param>
    public MobilePhoneNumber(string countryCode, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            throw new ArgumentException(string.Format(Resources.StringIsNullOrWhiteSpace, nameof(countryCode)), nameof(countryCode));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException(string.Format(Resources.StringIsNullOrWhiteSpace, nameof(phoneNumber)), nameof(phoneNumber));
        }

        countryCode = countryCode.Trim();
        phoneNumber = phoneNumber.Trim();

        if (!Regex.IsMatch(countryCode, @"^\d+$"))
            throw new FormatException("国家代码必须是数字。");
        if (!Regex.IsMatch(phoneNumber, @"^\d+$"))
            throw new FormatException("电话号码必须是数字。");

        this.CountryCode = countryCode;
        this.PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// 使用号码初始化移动电话号码。默认国家代码为86
    /// </summary>
    /// <param name="phoneNumber">电话号码。</param>
    public MobilePhoneNumber(string phoneNumber) : this(DefaultCountryCode, phoneNumber) { }

    /// <summary>
    /// 获取或设置国家代码。
    /// </summary>
    public string CountryCode { get; set; }

    /// <summary>
    /// 获取或设置电话号码。
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 已重写。按E.164格式输出移动电话号码。
    /// </summary>
    /// <returns></returns>
    public override readonly string ToString()
    {
        return $"+{this.CountryCode}{this.PhoneNumber}";
    }

    /// <summary>
    /// 尝试将给定字符串匹配为移动电话号码。
    /// </summary>
    /// <remarks>
    /// 此方法只支持中国（国家代码+86）的移动电话号码的匹配。
    /// </remarks>
    /// <param name="s">要进行匹配的字符串。</param>
    /// <param name="number">若匹配成功，则返回移动电话号码。</param>
    /// <returns>若匹配成功，返回true，否则返回false。</returns>
    public static bool TryParse(string? s, out MobilePhoneNumber number)
    {
        number = new MobilePhoneNumber();
        if (string.IsNullOrWhiteSpace(s))
            return false;
        s = s.Trim();

        var match = Regex.Match(s);
        if (!match.Success)
            return false;

        number = new MobilePhoneNumber(match.Result("${2}"));
        return true;
    }

    /// <summary>
    /// 匹配给定的字符串为移动电话号码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns>匹配成功的移动电话号码。</returns>
    /// <exception cref="FormatException">电话号码格式不正确。</exception>
    /// <exception cref="ArgumentException">字符串为空或null。</exception>
    public static MobilePhoneNumber Parse(string s)
    {
        return string.IsNullOrWhiteSpace(s)
            ? throw new ArgumentException(string.Format(Resources.StringIsNullOrWhiteSpace, nameof(s)), nameof(s))
            : !TryParse(s, out MobilePhoneNumber number) ? throw new FormatException("不正确的移动电话号码。") : number;
    }

    private static readonly Regex Regex = new(@"^(\+86)?(\d{11})$");
    private const string DefaultCountryCode = "86";


}
