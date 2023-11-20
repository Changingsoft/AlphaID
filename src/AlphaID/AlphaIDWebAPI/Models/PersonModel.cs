using IdSubjects;

namespace AlphaIdWebAPI.Models;



/// <summary>
/// 自然人
/// </summary>
/// <param name="SubjectId"> 主体Id. </param>
/// <param name="Name"> Name </param>
/// <param name="Sex"> 性别。 </param>
/// <param name="MobilePhoneSuffix"> 手机尾号 </param>
/// <param name="RealNameValid"> 获取一个值，指示是否已经过实名认证。 </param>
/// <param name="PhoneticSearchHint">  </param>
/// <param name="MembersOfHint">用户隶属组织的提示</param>
public record PersonModel(string SubjectId,
                          string Name,
                          string? Sex,
                          string? MobilePhoneSuffix,
                          bool RealNameValid,
                          string? PhoneticSearchHint,
                          IEnumerable<string>? MembersOfHint)
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="realNameValid"></param>
    /// <param name="members"></param>
    public PersonModel(NaturalPerson person, bool realNameValid = false,
                       IEnumerable<OrganizationMember>? members = null)
        : this(person.Id,
               person.PersonName.FullName,
               person.Gender.ToString(),
               person.PhoneNumber?.MobileSuffix(),
               realNameValid,
               person.PersonName.SearchHint,
               members?.Select(p => $"{p.Title} {p.Department} {p.Organization.Name}".Trim()))
    { }

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("此属性已废弃并始终返回false。客户端应取得realname范围来获取个人的实名制信息。")]
    public bool RealNameValid { get; set; } = false;
}
