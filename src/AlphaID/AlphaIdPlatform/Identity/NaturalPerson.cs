using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlphaIdPlatform.Identity;

/// <summary>
/// 表示一个自然人个体。
/// </summary>
public class NaturalPerson : ApplicationUser
{

    /// <summary>
    ///     个人经历。
    /// </summary>
    [MaxLength(200)]
    [PersonalData]
    public virtual string? Bio { get; set; }


    /// <summary>
    ///     姓氏拼音
    /// </summary>
    [PersonalData]
    [MaxLength(20)]
    [Unicode(false)]
    public virtual string? PhoneticSurname { get; set; }

    /// <summary>
    ///     名字拼音
    /// </summary>
    [PersonalData]
    [MaxLength(40)]
    [Unicode(false)]
    public virtual string? PhoneticGivenName { get; set; }

    /// <summary>
    ///     获取有关自然人更新的时间。当前未使用。
    /// </summary>
    [PersonalData]
    public virtual DateTimeOffset? PersonWhenChanged { get; set; }

    /// <summary>
    ///     检索提示，当以名称搜索时，搜索器将考虑检索提示的内容作为搜索参考。非拉丁语名字通常将注音名字作为检索提示，以方便在英文键盘状态下提高检索效率。
    /// </summary>
    [MaxLength(60)]
    public virtual string? SearchHint { get; set; }

    /// <summary>
    /// 银行账户。
    /// </summary>
    public virtual ICollection<NaturalPersonBankAccount> BankAccounts { get; set; } = [];
}
