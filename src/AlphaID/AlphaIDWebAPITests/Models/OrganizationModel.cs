namespace AlphaIDWebAPITests.Models;
internal record OrganizationModel(string? Domicile, string? Contact, string? LegalPersonName, string? USCI, DateTime? Expires)
{
    public string SubjectId { get; set; } = default!;
    public string Name { get; set; } = default!;
}
