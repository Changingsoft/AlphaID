namespace AlphaIDWebAPITests.Models;

internal record OrganizationMemberModel(string? Title, string? Department, string? Remark)
{
    public string PersonId { get; set; } = default!;

    public string PersonName { get; set; } = default!;

    public string OrganizationId { get; set; } = default!;

    public string OrganizationName { get; set; } = default!;
}
