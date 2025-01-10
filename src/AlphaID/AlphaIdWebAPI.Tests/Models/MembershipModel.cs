namespace AlphaIdWebAPI.Tests.Models;

internal record MembershipModel(string? Title, string? Department, string? Remark)
{
    public string PersonId { get; set; } = null!;

    public string PersonName { get; set; } = null!;

    public string OrganizationId { get; set; } = null!;

    public string OrganizationName { get; set; } = null!;
}