namespace AlphaIdWebAPI.Models;

/// <summary>
/// 身份证信息
/// </summary>
/// <param name="CardNumber"></param>
/// <param name="Name"></param>
/// <param name="Sex"></param>
/// <param name="DateOfBirth"></param>
/// <param name="Ethnicity"></param>
/// <param name="Address"></param>
/// <param name="Issuer"></param>
/// <param name="IssueDate"></param>
/// <param name="Expires"></param>
public record ChineseIdCardModel(string CardNumber,
                                 string Name,
                                 string Sex,
                                 DateTime DateOfBirth,
                                 string? Ethnicity,
                                 string? Address,
                                 string? Issuer,
                                 DateTime IssueDate,
                                 DateTime? Expires);
