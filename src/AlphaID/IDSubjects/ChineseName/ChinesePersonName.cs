using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.ChineseName;

/// <summary>
/// Chinese Person Name.
/// </summary>
[Owned]
public class ChinesePersonName
{
    /// <summary>
    /// 
    /// </summary>
    protected ChinesePersonName() { }

    /// <summary>
    /// 使用姓氏、名字和拼音初始化中国人名称。
    /// </summary>
    /// <param name="surname"></param>
    /// <param name="givenName"></param>
    /// <param name="phoneticSurname"></param>
    /// <param name="phoneticGivenName"></param>
    public ChinesePersonName(string? surname, string givenName, string? phoneticSurname, string phoneticGivenName)
    {
        this.Surname = surname;
        this.GivenName = givenName;
        this.PhoneticSurname = phoneticSurname;
        this.PhoneticGivenName = phoneticGivenName;
    }

    /// <summary>
    /// 姓氏。
    /// </summary>
    [MaxLength(10)]
    public string? Surname { get; protected set; }

    /// <summary>
    /// 名字（不含姓氏部分）。
    /// </summary>
    [MaxLength(10)]
    public string GivenName { get; protected set; } = default!;

    /// <summary>
    /// 姓名。
    /// </summary>
    [NotMapped]
    public string FullName => $"{this.Surname}{this.GivenName}";

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(20), Unicode(false)]
    public string? PhoneticSurname { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(40), Unicode(false)]
    public string PhoneticGivenName { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [NotMapped]
    public string PhoneticName => $"{this.PhoneticSurname} {this.PhoneticGivenName}".Trim();

    /// <summary>
    /// Override. Return full name of Person.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{this.FullName}|{this.PhoneticName}";
    }
}
