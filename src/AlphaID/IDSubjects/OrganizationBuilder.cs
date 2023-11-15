namespace IdSubjects;

/// <summary>
/// 组织构建器。
/// </summary>
public class OrganizationBuilder
{

    /// <summary>
    /// 使用组织名称初始化组织构建器。
    /// </summary>
    /// <param name="name"></param>
    public OrganizationBuilder(string name)
    {
        this.Organization = new GenericOrganization()
        {
            Name = name.Trim(),
        };
    }

    /// <summary>
    /// 获取此构建器所构建的组织。
    /// </summary>
    public GenericOrganization Organization { get; }
}
