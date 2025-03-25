using RadiusCore.Models;
using System.Net;

namespace RadiusCore;

internal class RadiusClientManager(IRadiusClientStore radiusClientStore)
{
    public Task<RadiusClient?> FindByEndpoint(IPEndPoint endpoint)
    {
        return Task.FromResult(radiusClientStore.RadiusClients.FirstOrDefault(c => c.Address == endpoint.Address.ToString()));
    }
}