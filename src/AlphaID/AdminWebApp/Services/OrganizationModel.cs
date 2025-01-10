namespace AdminWebApp.Services;

public class OrganizationModel
{
    public string SubjectId { get; set; } = null!;
    public string Name { get; set; } = null!;

    public string? Domicile { get; set; }

    public string? Contact { get; set; }

    public DateTime? Expires { get; set; }
}