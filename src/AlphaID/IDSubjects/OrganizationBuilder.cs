using IDSubjects.Subjects;

namespace IDSubjects;

/// <summary>
/// 组织构建器。
/// </summary>
public class OrganizationBuilder
{

    /// <summary>
    /// 使用已知的组织信息初始化构建器。
    /// </summary>
    /// <param name="org"></param>
    public OrganizationBuilder(GenericOrganization org)
    {
        this.Organization = org ?? throw new ArgumentNullException(nameof(org));
    }

    /// <summary>
    /// 使用组织名称初始化组织构建器。
    /// </summary>
    /// <param name="name"></param>
    public OrganizationBuilder(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"“{nameof(name)}”不能为 null 或空白。", nameof(name));
        }

        this.Organization = new GenericOrganization()
        {
            Name = name.Trim(),
        };
    }

    /// <summary>
    /// 设置统一社会信用代码。
    /// </summary>
    /// <param name="uscc"></param>
    /// <returns></returns>
    public OrganizationBuilder SetUsci(Uscc uscc)
    {
        this.Organization.Usci = uscc.ToString();

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public OrganizationBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"“{nameof(name)}”不能为 null 或空白。", nameof(name));
        }

        this.Organization.Name = name.Trim();
        return this;
    }

    /// <summary>
    /// 获取此构建器所构建的组织。
    /// </summary>
    public GenericOrganization Organization { get; }
}
