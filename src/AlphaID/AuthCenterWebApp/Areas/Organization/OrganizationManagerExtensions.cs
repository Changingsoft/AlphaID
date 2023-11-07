using IDSubjects;

namespace AuthCenterWebApp.Areas.Organization;

public static class OrganizationManagerExtensions
{
    public static async Task<IEnumerable<GenericOrganization>> FindByAnchorAsync(this OrganizationManager manager, string anchor)
    {
        //Find by name first.
        var orgs = manager.SearchByName(anchor).ToList();
        //Find by ID if list is empty.
        if (!orgs.Any())
        {
            var org = await manager.FindByIdAsync(anchor);
            if (org != null)
                orgs.Add(org);
        }
        return orgs;
    }

    public static bool TryGetSingleOrDefaultOrganization(this OrganizationManager manager, string anchor,
        out GenericOrganization? organization)
    {
        var orgs = manager.SearchByName(anchor).ToList();
        if (!orgs.Any())
        {
            organization = default!;
            return true;
        }
        if (orgs.Count != 1)
        {
            organization = default!;
            return false;
        }

        organization = orgs.Single();
        return true;

    }
}
