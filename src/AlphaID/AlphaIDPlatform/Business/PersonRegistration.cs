using IDSubjects;
using IDSubjects.RealName;
using IDSubjects.Subjects;

namespace AlphaIDPlatform.Business;

/// <summary>
/// 自然人注册信息。
/// </summary>
public class PersonRegistration
{
    /// <summary>
    /// 使用姓名、移动电话和性别（可选）创建一个自然人登记表。
    /// </summary>
    /// <param name="displayName"></param>
    /// <param name="mobile"></param>
    /// <param name="sex"></param>
    public PersonRegistration(string displayName, MobilePhoneNumber mobile, Gender? sex = null)
    {
        this.DisplayName = displayName;
        this.Mobile = mobile;
        this.Sex = sex;
        this.OrganizationMembers = new HashSet<OrganizationMemberDescriptor>();
    }

    /// <summary>
    /// 身份证。
    /// </summary>
    public ChineseIDCardInfo? ChineseIdCard { get; internal set; }

    /// <summary>
    /// 姓氏。
    /// </summary>
    public string? Surname { get; internal set; }

    /// <summary>
    /// 名字。
    /// </summary>
    public string? GivenName { get; internal set; }

    /// <summary>
    /// 显示名称。
    /// </summary>
    public string DisplayName { get; internal set; }

    /// <summary>
    /// 拼音名称。
    /// </summary>
    public string? PhoneticDisplayName { get; internal set; }

    /// <summary>
    /// 移动电话。
    /// </summary>
    public MobilePhoneNumber? Mobile { get; internal set; }

    /// <summary>
    /// 登录账号描述
    /// </summary>
    public LoginAccountDescriptor? LoginAccount { get; internal set; }

    /// <summary>
    /// 启用邮件服务。
    /// </summary>
    public bool EnableMailService { get; internal set; }

    /// <summary>
    /// 性别。
    /// </summary>
    public Gender? Sex { get; internal set; }

    /// <summary>
    /// 要加入的组织。
    /// </summary>
    public ICollection<OrganizationMemberDescriptor> OrganizationMembers { get; }
}