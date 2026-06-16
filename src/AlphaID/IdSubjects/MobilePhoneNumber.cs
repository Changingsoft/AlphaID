using System.Text.RegularExpressions;

namespace IdSubjects;

/// <summary>
/// 手机号。
/// </summary>
public struct MobilePhoneNumber
{
    /// <summary>
    /// 使用国家代码和手机号初始化。
    /// </summary>
    /// <param name="countryCode">国家代码。</param>
    /// <param name="phoneNumber">电话号码。</param>
    public MobilePhoneNumber(string countryCode, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException(string.Format(Resources.StringIsNullOrWhiteSpace, nameof(countryCode)),
                nameof(countryCode));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException(string.Format(Resources.StringIsNullOrWhiteSpace, nameof(phoneNumber)),
                nameof(phoneNumber));

        countryCode = countryCode.Trim();
        phoneNumber = phoneNumber.Trim();

        if (!Regex.IsMatch(countryCode, @"^\d+$"))
            throw new FormatException("国家代码必须是数字。");
        if (!Regex.IsMatch(phoneNumber, @"^\d+$"))
            throw new FormatException("电话号码必须是数字。");

        CountryCode = countryCode;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// 使用号码初始化手机号。默认国家代码为86
    /// </summary>
    /// <param name="phoneNumber">电话号码。</param>
    public MobilePhoneNumber(string phoneNumber) : this(DefaultCountryCode, phoneNumber)
    {
    }

    /// <summary>
    /// 获取或设置国家代码。
    /// </summary>
    public string CountryCode { get; set; }

    /// <summary>
    /// 获取或设置电话号码。
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 已重写。按 E.164 格式输出手机号。
    /// </summary>
    /// <returns></returns>
    public readonly override string ToString()
    {
        return $"+{CountryCode}{PhoneNumber}";
    }

    /// <summary>
    /// 尝试将给定字符串匹配为手机号。
    /// </summary>
    /// <remarks>
    /// 此方法支持全球手机号的匹配。对于不带国家代码的11位数字，将默认视为中国大陆手机号。
    /// </remarks>
    /// <param name="s">要进行匹配的字符串。</param>
    /// <param name="number">若匹配成功，则返回手机号。</param>
    /// <returns>若匹配成功，返回true，否则返回false。</returns>
    public static bool TryParse(string? s, out MobilePhoneNumber number)
    {
        number = new MobilePhoneNumber();
        if (string.IsNullOrWhiteSpace(s))
            return false;
        s = s.Trim();

        string numberToParse;
        if (s.StartsWith("+"))
        {
            numberToParse = s[1..];
        }
        else
        {
            // For backward compatibility, if it's 11 digits, assume it's a Chinese number.
            if (Regex.IsMatch(s, @"^\d{11}$"))
            {
                number = new MobilePhoneNumber(DefaultCountryCode, s);
                return true;
            }
            numberToParse = s;
        }

        if (!Regex.IsMatch(numberToParse, @"^\d+$"))
            return false;

        foreach (var code in s_countryCodes)
        {
            if (numberToParse.StartsWith(code))
            {
                var phoneNumber = numberToParse[code.Length..];
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    number = new MobilePhoneNumber(code, phoneNumber);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 匹配给定的字符串为手机号。
    /// </summary>
    /// <param name="s"></param>
    /// <returns>匹配成功的手机号。</returns>
    /// <exception cref="FormatException">电话号码格式不正确。</exception>
    /// <exception cref="ArgumentException">字符串为空或null。</exception>
    public static MobilePhoneNumber Parse(string s)
    {
        return string.IsNullOrWhiteSpace(s)
            ? throw new ArgumentException(string.Format(Resources.StringIsNullOrWhiteSpace, nameof(s)), nameof(s))
            : !TryParse(s, out MobilePhoneNumber number)
                ? throw new FormatException("不正确的手机号。")
                : number;
    }

    private static readonly string[] s_countryCodes =
    [
        // The data is from https://www.itu.int/dms_pub/itu-t/opb/sp/T-SP-E.164D-2016-PDF-E.pdf
        // and https://www.itu.int/dms_pub/itu-t/opb/sp/T-SP-E.164C-2011-PDF-E.pdf
        // and https://en.wikipedia.org/wiki/List_of_country_calling_codes
        "1242", "1246", "1264", "1268", "1284", "1340", "1345", "1441", "1473", "1649", "1664", "1670", "1671",
        "1684", "1721", "1758", "1767", "1784", "1809", "1829", "1849", "1868", "1869", "1876", "991", "979",
        "881", "878", "870", "808", "800", "692", "691", "690", "689", "688", "687", "686", "685", "683", "682",
        "681", "680", "679", "678", "677", "676", "675", "674", "673", "672", "670", "599", "598", "597", "596",
        "595", "594", "593", "592", "591", "590", "509", "508", "507", "506", "505", "504", "503", "502", "501",
        "500", "423", "421", "420", "389", "387", "386", "385", "383", "382", "381", "380", "379", "378", "377",
        "376", "375", "374", "373", "372", "371", "370", "359", "358", "357", "356", "355", "354", "353", "352",
        "351", "350", "299", "298", "297", "291", "290", "269", "268", "267", "266", "265", "264", "263", "262",
        "261", "260", "258", "257", "256", "255", "254", "253", "252", "251", "250", "249", "248", "246", "245",
        "244", "243", "242", "241", "240", "239", "238", "237", "236", "235", "234", "233", "232", "231", "230",
        "229", "228", "227", "226", "225", "224", "223", "222", "221", "220", "218", "216", "213", "212", "211",
        "98", "95", "94", "93", "92", "91", "90", "86", "84", "82", "81", "66", "65", "64", "63", "62", "61", "60",
        "58", "57", "56", "55", "54", "53", "52", "51", "49", "48", "47", "46", "45", "44", "43", "41", "40", "39",
        "36", "34", "33", "32", "31", "30", "27", "20", "7", "1"
    ];
    private const string DefaultCountryCode = "86";
}