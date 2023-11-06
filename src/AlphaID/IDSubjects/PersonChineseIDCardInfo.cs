using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IDSubjects;

/// <summary>
/// 中国居民身份证。
/// </summary>
[Owned]
public class PersonChineseIdCardInfo
{
    /// <summary>
    /// 
    /// </summary>
    protected PersonChineseIdCardInfo() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cardNumber"></param>
    /// <param name="name"></param>
    /// <param name="ethnicity"></param>
    /// <param name="address"></param>
    public PersonChineseIdCardInfo(string cardNumber,
                                 string name,
                                 string ethnicity,
                                 string address)
    {
        this.CardNumber = cardNumber;
        this.Name = name;
        this.Ethnicity = ethnicity;
        this.Address = address;
    }

    /// <summary>
    /// Identifier Value.
    /// </summary>
    [MaxLength(18), Unicode(false)]
    [Required(ErrorMessage = "Validate_Required")]
    public string CardNumber { get; set; } = default!;

    /// <summary>
    /// 姓名
    /// </summary>
    [MaxLength(50)]
    public string Name { get; protected set; } = default!;


    /// <summary>
    /// 民族。
    /// </summary>
    [MaxLength(50)]
    public string? Ethnicity { get; protected set; } = default!;


    /// <summary>
    /// 住址。
    /// </summary>
    [MaxLength(100)]
    public string? Address { get; protected set; } = default!;

}
