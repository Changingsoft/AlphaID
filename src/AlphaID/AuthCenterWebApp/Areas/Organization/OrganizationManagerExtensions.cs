using IdSubjects;

namespace AuthCenterWebApp.Areas.Organization;

public static class OrganizationManagerExtensions
{
    public static async Task<IEnumerable<IdSubjects.Organization>> FindByAnchorAsync(this OrganizationManager manager,
        string anchor)
    {
        //Find by name first.
        IdSubjects.Organization[] orgs = manager.FindByName(anchor).ToArray();
        //Find by ID if list is empty.
        if (orgs.Length != 0) return orgs;
        IdSubjects.Organization? org = await manager.FindByIdAsync(anchor);
        return org != null ? [org] : [];
    }

    public static bool TryGetSingleOrDefaultOrganization(this OrganizationManager manager,
        string anchor,
        out IdSubjects.Organization? organization)
    {
        if (manager.TryFindSingleOrDefaultByName(anchor, out organization))
        {
            organization ??= manager.FindById(anchor);
            return true;
        }

        return false;
    }
}