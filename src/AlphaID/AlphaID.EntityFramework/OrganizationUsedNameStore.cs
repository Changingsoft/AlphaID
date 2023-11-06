using IDSubjects;

namespace AlphaID.EntityFramework;
public class OrganizationUsedNameStore : IQueryableOrganizationUsedNameStore
{
    private readonly IdSubjectsDbContext dbContext;

    public OrganizationUsedNameStore(IdSubjectsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<OrganizationUsedName> UsedNames => this.dbContext.OrganizationUsedNames;
}
