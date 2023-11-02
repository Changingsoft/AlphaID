using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IDSubjects.RealName;

/// <summary>
/// 身份证扫描件
/// </summary>
[Owned]
public class ChineseIDCardImage
{
    /// <summary>
    /// 
    /// </summary>
    protected ChineseIDCardImage() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personalFace"></param>
    /// <param name="personalFaceMimeType"></param>
    /// <param name="issuerFace"></param>
    /// <param name="issuerFaceMimeType"></param>
    public ChineseIDCardImage(byte[] personalFace, string personalFaceMimeType, byte[] issuerFace, string issuerFaceMimeType)
    {
        this.PersonalFace = personalFace;
        this.IssuerFace = issuerFace;
        this.PersonalFaceMimeType = personalFaceMimeType;
        this.IssuerFaceMimeType = issuerFaceMimeType;
    }

    /// <summary>
    /// 个人信息页面。
    /// </summary>
    public byte[] PersonalFace { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string PersonalFaceMimeType { get; set; } = default!;

    /// <summary>
    /// 国徽/颁发者页面。
    /// </summary>
    public byte[] IssuerFace { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string IssuerFaceMimeType { get; set; } = default!;
}
