namespace AlphaIdPlatform.Platform;

/// <summary>
///     GenericOrganization info.
/// </summary>
/// <remarks>
///     ctor.
/// </remarks>
/// <param name="uSci"></param>
/// <param name="name"></param>
/// <param name="domicile"></param>
/// <param name="phoneNumber"></param>
/// <param name="email"></param>
/// <param name="organizationCode"></param>
/// <param name="taxNumber"></param>
public class OrganizationInfo(
    string uSci,
    string name,
    string domicile,
    string phoneNumber,
    string email,
    string organizationCode,
    string taxNumber)
{
    /// <summary>
    ///     统一社会信用代码
    /// </summary>
    public string Usci { get; protected internal set; } = uSci;

    /// <summary>
    ///     名称
    /// </summary>
    public string Name { get; protected internal set; } = name;

    /// <summary>
    ///     住所
    /// </summary>
    public string Domicile { get; protected internal set; } = domicile;

    /// <summary>
    ///     联系电话
    /// </summary>
    public string PhoneNumber { get; protected internal set; } = phoneNumber;

    /// <summary>
    ///     邮件
    /// </summary>
    public string Email { get; protected internal set; } = email;

    /// <summary>
    ///     组织机构代码
    /// </summary>
    public string OrganizationCode { get; protected internal set; } = organizationCode;

    /// <summary>
    ///     纳税人识别号。
    /// </summary>
    public string TaxNumber { get; protected internal set; } = taxNumber;
}