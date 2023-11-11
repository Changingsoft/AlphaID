using System.ComponentModel.DataAnnotations;

namespace IDSubjects;

/// <summary>
/// 
/// </summary>
public enum OrganizationIdentifierType
{
    /// <summary>
    /// 统一社会信用代码。
    /// </summary>
    [Display(Name = "Unified Social Credit Code", ResourceType = typeof(Resources))]
    UnifiedSocialCreditCode,
    /// <summary>
    /// 全球法人实体标识符（LEI）
    /// </summary>
    [Display(Name = "Legal Entity Identifier", ResourceType = typeof(Resources))]
    LegalEntityIdentifier,
    /// <summary>
    /// 邓白氏编码
    /// </summary>
    [Display(Name = "Duns Number", ResourceType = typeof(Resources))]
    DunsNumber,
}