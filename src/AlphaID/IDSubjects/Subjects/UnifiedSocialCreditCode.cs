using System.Text.RegularExpressions;

namespace IdSubjects.Subjects;

/// <summary>
/// 表示一个统一社会信用代码。
/// </summary>
public readonly struct UnifiedSocialCreditCode
{

    /// <summary>
    /// 使用指定的代码值初始化统一社会信用代码。
    /// </summary>
    /// <param name="code"></param>
    public UnifiedSocialCreditCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("数据为空。");
        code = code.Trim().ToUpper();
        if (code.Length != 17)
            throw new ArgumentException("编码长度不符。");

        var adminCode = code[..1];
        var orgTypeCode = code.Substring(1, 1);
        var regionCode = code.Substring(2, 6);
        var organizationCode = code.Substring(8, 9);

        adminCode = adminCode.Trim().ToUpper();
        orgTypeCode = orgTypeCode.Trim().ToUpper();
        regionCode = regionCode.Trim().ToUpper();

        if (!Regex.IsMatch(adminCode, @"^[0-9,A-H,J-N,P-R,T-U,W-Y]{1}$"))
            throw new ArgumentException("登记管理部门代码无效。");
        if (!Regex.IsMatch(orgTypeCode, @"^[0-9,A-H,J-N,P-R,T-U,W-Y]{1}$"))
            throw new ArgumentException("机构类别代码无效。");
        if (!Regex.IsMatch(regionCode, @"^\d{6}$"))
            throw new ArgumentException("登记管理机关行政区划代码无效。");

        this.AdminCode = adminCode;
        this.OrganizationTypeCode = orgTypeCode;
        this.RegionCode = regionCode;
        this.OrganizationCode = OrganizationCode.Parse(organizationCode);
        this.CheckCode = Check(this.AdminCode + this.OrganizationTypeCode + this.RegionCode + this.OrganizationCode.ToString(true));
    }

    /// <summary>
    /// 使用给定的组合代码初始化统一社会信用代码。
    /// </summary>
    /// <param name="adminCode"></param>
    /// <param name="orgTypeCode"></param>
    /// <param name="regionCode"></param>
    /// <param name="organizationCode"></param>
    public UnifiedSocialCreditCode(string adminCode, string orgTypeCode, string regionCode, string organizationCode)
        : this(adminCode, orgTypeCode, regionCode, OrganizationCode.Parse(organizationCode))
    { }

    /// <summary>
    /// 使用给定的组合代码和组织机构代码初始化统一社会信用代码。
    /// </summary>
    /// <param name="adminCode"></param>
    /// <param name="orgTypeCode"></param>
    /// <param name="regionCode"></param>
    /// <param name="organizationCode"></param>
    public UnifiedSocialCreditCode(string adminCode, string orgTypeCode, string regionCode, OrganizationCode organizationCode)
    {
        adminCode = adminCode.Trim().ToUpper();
        orgTypeCode = orgTypeCode.Trim().ToUpper();
        regionCode = regionCode.Trim().ToUpper();

        if (!Regex.IsMatch(adminCode, @"^[0-9,A-H,J-N,P-R,T-U,W-Y]{1}$"))
            throw new ArgumentException(Resources.Invalid_administrative_code, nameof(adminCode));
        if (!Regex.IsMatch(orgTypeCode, @"^[0-9,A-H,J-N,P-R,T-U,W-Y]{1}$"))
            throw new ArgumentException(Resources.Invlid_organization_type_, nameof(orgTypeCode));
        if (!Regex.IsMatch(regionCode, @"^\d{6}$"))
            throw new ArgumentException(Resources.Invalid_region_code_, nameof(regionCode));

        this.AdminCode = adminCode;
        this.OrganizationTypeCode = orgTypeCode;
        this.RegionCode = regionCode;
        this.OrganizationCode = organizationCode;
        this.CheckCode = Check(this.AdminCode + this.OrganizationTypeCode + this.RegionCode + this.OrganizationCode.ToString(true));
    }

    /// <summary>
    /// 获取该统一社会信用代码的编码部分（不包括校验码）。
    /// </summary>
    public string Code
    {
        get
        {
            return this.AdminCode + this.OrganizationTypeCode + this.RegionCode + this.OrganizationCode.ToString(true);
        }
    }

    /// <summary>
    /// 获取登记管理部门代码。
    /// </summary>
    public string AdminCode { get; }

    /// <summary>
    /// 获取组织类别代码。
    /// </summary>
    public string OrganizationTypeCode { get; }

    /// <summary>
    /// 获取登记机关行政区划代码。
    /// </summary>
    public string RegionCode { get; }

    /// <summary>
    /// 获取主体标识码（组织机构代码）
    /// </summary>
    public OrganizationCode OrganizationCode { get; }

    /// <summary>
    /// 获取校验码。
    /// </summary>
    public char CheckCode { get; }

    /// <summary>
    /// 已重写，输出统一社会信用代码的字符串形式。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.AdminCode + this.OrganizationTypeCode + this.RegionCode + this.OrganizationCode.ToString(true) + this.CheckCode;
    }

    /// <summary>
    /// 已重写，输出统一社会信用代码的哈希。
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }

    /// <summary>
    /// 已重写。比较两个统一社会信用代码是否值相等。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return obj is UnifiedSocialCreditCode uScc ? this == uScc : base.Equals(obj);
    }

    /// <summary>
    /// 已重载。变更相等比较为值相等比较。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(UnifiedSocialCreditCode a, UnifiedSocialCreditCode b)
    {
        return a.AdminCode == b.AdminCode
            && a.OrganizationTypeCode == b.OrganizationTypeCode
            && a.RegionCode == b.RegionCode
            && a.OrganizationCode == b.OrganizationCode
            && a.CheckCode == b.CheckCode;
    }

    /// <summary>
    /// 已重载。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(UnifiedSocialCreditCode a, UnifiedSocialCreditCode b)
    {
        return !(a == b);
    }

    private static char Check(string value)
    {
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            var charIndex = Charset.IndexOf(value[i]);
            if (charIndex < 0)
                throw new ArgumentException("无效字符");
            sum += charIndex * Weight[i];
        }
        return Charset[(31 - (sum % 31)) % 31]; //处理当余数为0时，31-0 = 31，超出字符集范围，再次取模得0，约束在 0-30 范围内。
    }

    /// <summary>
    /// Charset with order from index 0 to 30,
    /// </summary>
    private const string Charset = "0123456789ABCDEFGHJKLMNPQRTUWXY";

    /// <summary>
    /// Readonly weight for corresponding position.
    /// </summary>
    private static readonly int[] Weight = [1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28];

    #region Public static methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static UnifiedSocialCreditCode Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentNullException(nameof(s));

        s = s.Trim().ToUpper();
        if (s.Length != 18)
            throw new ArgumentException("Length incorrect.");

        string codePart = s[..17];
        char checkCode = s[17];

        UnifiedSocialCreditCode newUsci = new(codePart);
        return newUsci.CheckCode != checkCode ? throw new ArgumentException("Checksum incorrect.") : newUsci;
    }

    /// <summary>
    /// 通过给定的输入尝试匹配为一个统一社会信用代码。
    /// </summary>
    /// <param name="s"></param>
    /// <param name="usci"></param>
    /// <returns>若匹配成功，返回true，否则返回false.</returns>
    public static bool TryParse(string s, out UnifiedSocialCreditCode usci)
    {
        usci = new UnifiedSocialCreditCode();
        if (string.IsNullOrWhiteSpace(s))
            return false;

        s = s.Trim().ToUpper();
        if (s.Length != 18)
            return false;

        //Check if contained any invalid char.
        foreach (var c in s)
        {
            if (Charset.IndexOf(c) < 0)
                return false;
        }

        string codePart = s[..17];
        var adminCode = codePart[..1];
        var orgTypeCode = codePart.Substring(1, 1);
        var regionCode = codePart.Substring(2, 6);
        var organizationCode = codePart.Substring(8, 9);
        char checkCode = s[17];

        if (!OrganizationCode.TryParse(organizationCode, out OrganizationCode orgCode))
            return false;

        UnifiedSocialCreditCode innerUscc = new(adminCode, orgTypeCode, regionCode, orgCode);
        if (innerUscc.CheckCode != checkCode)
            return false;

        usci = innerUscc;
        return true;
    }

    /// <summary>
    /// 判定给定的输入是否是一个格式上合法的统一社会性用代码。
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsUsci(string s)
    {
        return TryParse(s, out _);
    }

    #endregion
}
