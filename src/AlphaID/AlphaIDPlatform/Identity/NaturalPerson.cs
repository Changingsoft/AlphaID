using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

}
