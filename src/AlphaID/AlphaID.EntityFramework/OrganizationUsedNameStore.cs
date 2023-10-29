using IDSubjects;

namespace AlphaID.EntityFramework;
public class OrganizationUsedNameStore : IQueryableOrganizationUsedNameStore
{
    private readonly IDSubjectsDbContext dbContext;

    public OrganizationUsedNameStore(IDSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<OrganizationUsedName> UsedNames => this.dbContext.OrganizationUsedNames;
}
