namespace AlphaIDPlatform.Business;

/// <summary>
/// 组织成员描述符。
/// </summary>
public class OrganizationMemberDescriptor
{
    /// <summary>
    /// Initialize.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="department"></param>
    /// <param name="title"></param>
    public OrganizationMemberDescriptor(string organizationId, string department, string title)
    {
        this.OrganizationId = organizationId;
        this.Department = department;
        this.Title = title;
    }

    /// <summary>
    /// GenericOrganization Id.
    /// </summary>
    public string OrganizationId { get; set; }

    /// <summary>
    /// Department.
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// Title.
    /// </summary>
    public string Title { get; set; }
}
