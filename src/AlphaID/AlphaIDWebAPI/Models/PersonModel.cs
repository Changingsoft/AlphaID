using IDSubjects;

namespace AlphaIDWebAPI.Models;



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
               person.Name,
               person.Sex.ToString(),
               person.PhoneNumber?.MobileSuffix(),
               realNameValid,
               person.PhoneticSearchHint,
               members?.Select(p => $"{p.Title} {p.Department} {p.Organization.Name}".Trim()))
    { }
}
