namespace AdminWebApp.Services;

public class OrganizationModel
{
    public string SubjectId { get; set; } = default!;
    public string Name { get; set; } = default!;

    public string? Domicile { get; set; }

    public string? Contact { get; set; }

    public string? Usci { get; set; }

    public DateTime? Expires { get; set; }
}