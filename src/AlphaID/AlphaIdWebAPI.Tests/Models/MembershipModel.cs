namespace AlphaIdWebAPI.Tests.Models;

internal record MembershipModel(string? Title, string? Department, string? Remark)
{
    public string PersonId { get; set; } = default!;

    public string PersonName { get; set; } = default!;

    public string OrganizationId { get; set; } = default!;

    public string OrganizationName { get; set; } = default!;
}
