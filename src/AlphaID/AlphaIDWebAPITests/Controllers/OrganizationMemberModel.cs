namespace AlphaIDWebAPITests.Controllers;
internal class OrganizationMemberModel
{
    public string PersonId { get; set; } = default!;

    public string PersonName { get; set; } = default!;

    public string OrganizationId { get; set; } = default!;

    public string OrganizationName { get; set; } = default!;

    public string? Title { get; set; }

    public string? Department { get; set; }

    public string? Remark { get; set; }
}
