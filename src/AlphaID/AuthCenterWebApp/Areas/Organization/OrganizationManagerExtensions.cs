using IDSubjects;

namespace AuthCenterWebApp.Areas.Organization;

public static class OrganizationManagerExtensions
{
    public static async Task<IEnumerable<GenericOrganization>> FindByAnchorAsync(this OrganizationManager manager, string anchor)
    {
        //Find by name first.
        var orgs = (await manager.SearchByNameAsync(anchor)).ToList();
        //Find by ID if list is empty.
        if(!orgs.Any())
        {
            var org = await manager.FindByIdAsync(anchor);
            if(org != null)
                orgs.Add(org);
        }
        return orgs;
    }
}
