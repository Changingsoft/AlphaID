namespace AlphaIDWebAPITests.Controllers;
internal class OrganizationModel
{
    public string SubjectId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Domicile { get; set; }
    public string? Contact { get; set; }
    public string? LegalPersonName { get; set; }
    public string? USCI { get; set; }
    public DateTime? Expires { get; set; }
}
