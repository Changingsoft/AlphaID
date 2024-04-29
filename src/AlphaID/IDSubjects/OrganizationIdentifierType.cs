using System.ComponentModel.DataAnnotations;

namespace IdSubjects;

/// <summary>
/// </summary>
public enum OrganizationIdentifierType
{
    /// <summary>
    ///     统一社会信用代码。
    /// </summary>
    [Display(Name = "UnifiedSocialCreditCode", ResourceType = typeof(Resources))]
    UnifiedSocialCreditCode,

    /// <summary>
    ///     全球法人实体标识符（LEI）
    /// </summary>
    [Display(Name = "Legal_entity_identifier", ResourceType = typeof(Resources))]
    LegalEntityIdentifier,

    /// <summary>
    ///     邓白氏编码
    /// </summary>
    [Display(Name = "DunsNumber", ResourceType = typeof(Resources))]
    DunsNumber
}