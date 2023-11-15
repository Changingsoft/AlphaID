using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdSubjects;

/// <summary>
/// 表示个人名称。
/// </summary>
[Owned]
[Index(nameof(FullName))]
[Index(nameof(SearchHint))]
public class PersonNameInfo
{
    /// <summary>
    /// For persistence.
    /// </summary>
    protected PersonNameInfo() { }

    /// <summary>
    /// Create a PersonNameInfo.
    /// </summary>
    /// <param name="fullName"></param>
    /// <param name="surname"></param>
    /// <param name="givenName"></param>
    /// <param name="middleName"></param>
    public PersonNameInfo(string fullName, string? surname = null, string? givenName = null, string? middleName = null)
    {
        this.Surname = surname;
        this.GivenName = givenName;
        this.FullName = fullName;
        this.MiddleName = middleName;
    }

    /// <summary>
    /// 
    /// </summary>
    [PersonalData]
    [MaxLength(50)]
    public string? Surname { get; protected set; }

    /// <summary>
    /// Middle name.
    /// </summary>
    [PersonalData]
    [MaxLength(50)]
    public string? MiddleName { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    [PersonalData]
    [MaxLength(50)]
    public string? GivenName { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    [PersonalData]
    [MaxLength(150)]
    public string FullName { get; protected set; } = default!;

    /// <summary>
    /// 检索提示，当以名称搜索时，搜索器将考虑检索提示的内容作为搜索参考。非拉丁语名字通常将注音名字作为检索提示，以方便在英文键盘状态下提高检索效率。
    /// </summary>
    [MaxLength(60)]
    public virtual string? SearchHint { get; set; }

}