using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaIdPlatform.JoinOrgRequesting;

namespace AlphaId.EntityFramework;
internal class JoinOrganizationRequestStore(AlphaIdDbContext dbContext) : IJoinOrganizationRequestStore
{
    public IQueryable<JoinOrganizationRequest> Requests => dbContext.JoinOrganizationRequests;

    public async Task CreateAsync(JoinOrganizationRequest item)
    {
        dbContext.JoinOrganizationRequests.Add(item);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(JoinOrganizationRequest item)
    {
        dbContext.JoinOrganizationRequests.Update(item);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(JoinOrganizationRequest item)
    {
        dbContext.JoinOrganizationRequests.Remove(item);
        await dbContext.SaveChangesAsync();
    }
}
