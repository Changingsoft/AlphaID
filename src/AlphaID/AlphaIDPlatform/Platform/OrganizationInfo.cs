namespace AlphaIDPlatform.Platform;

/// <summary>
/// GenericOrganization info.
/// </summary>
public class OrganizationInfo
{
    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="uSCI"></param>
    /// <param name="name"></param>
    /// <param name="domicile"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="email"></param>
    /// <param name="organizationCode"></param>
    /// <param name="taxNumber"></param>
    public OrganizationInfo(string uSCI, string name, string domicile, string phoneNumber, string email, string organizationCode, string taxNumber)
    {
        this.USCI = uSCI;
        this.Name = name;
        this.Domicile = domicile;
        this.PhoneNumber = phoneNumber;
        this.Email = email;
        this.OrganizationCode = organizationCode;
        this.TaxNumber = taxNumber;
    }

    /// <summary>
    /// 统一社会信用代码
    /// </summary>
    public string USCI { get; protected internal set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; protected internal set; }

    /// <summary>
    /// 住所
    /// </summary>
    public string Domicile { get; protected internal set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string PhoneNumber { get; protected internal set; }

    /// <summary>
    /// 邮件
    /// </summary>
    public string Email { get; protected internal set; }

    /// <summary>
    /// 组织机构代码
    /// </summary>
    public string OrganizationCode { get; protected internal set; }

    /// <summary>
    /// 纳税人识别号。
    /// </summary>
    public string TaxNumber { get; protected internal set; }
}