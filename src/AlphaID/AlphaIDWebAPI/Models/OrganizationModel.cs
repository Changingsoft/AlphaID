using IDSubjects;

namespace AlphaIDWebAPI.Models;

/// <summary>
/// GenericOrganization.
/// </summary>
/// <param name="SubjectId">组织的Id</param>
/// <param name="Name">组织的名称</param>
/// <param name="Domicile">住所</param>
/// <param name="Contact">联系方式</param>
/// <param name="LegalPersonName">组织的负责人或代表人名称</param>
/// <param name="USCI">统一社会信用代码</param>
/// <param name="Expires">有效期</param>
public record OrganizationModel(string SubjectId,
                                string Name,
                                string? Domicile,
                                string? Contact,
                                string? LegalPersonName,
                                string? USCI,
                                DateTime? Expires)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="organization"></param>
    public OrganizationModel(GenericOrganization organization)
        : this(organization.Id,
               organization.Name,
               organization.Domicile,
               organization.Contact,
               organization.LegalPersonName,
               organization.USCI,
               organization.TermEnd)
    {
    }
}
