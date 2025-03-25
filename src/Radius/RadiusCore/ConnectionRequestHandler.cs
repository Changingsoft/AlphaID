
using RadiusCore.Models;

namespace RadiusCore;

internal class ConnectionRequestHandler(RadiusClientManager radiusClientManager, NetworkPolicyHandlerFactory networkPolicyHandlerFactory)
{
    internal async Task HandleAsync(RadiusContext radiusContext)
    {
        if(radiusContext.ConnectionRequestPolicy == null)
            throw new InvalidOperationException("ConnectionRequestPolicy is not set.");

        switch (radiusContext.ConnectionRequestPolicy.RouteType)
        {
            case RouteType.Forward:
                await ForwardAsync(radiusContext);
                break;
            case RouteType.Local:
                await LocalAsync(radiusContext);
                break;
            case RouteType.Bypass:
                await BypassAsync(radiusContext);
                break;
        }
    }

    private async Task BypassAsync(RadiusContext radiusContext)
    {
        throw new NotImplementedException();
    }

    private async Task LocalAsync(RadiusContext radiusContext)
    {
        var client = await radiusClientManager.FindByEndpoint(radiusContext.Request.Remote);
        if (client == null)
        {
            //todo Send reject request.
        }
        radiusContext.Client = client;

        var networkPolicyHandler = networkPolicyHandlerFactory.CreateHandler(radiusContext);
        await networkPolicyHandler.HandleAsync(radiusContext);
    }

    private async Task ForwardAsync(RadiusContext radiusContext)
    {
        if(!radiusContext.ConnectionRequestPolicy!.RemoteServerGroupId.HasValue)
            throw new InvalidOperationException("RemoteServerGroup is not set.");

        //todo 载入远程服务器组，选择某个服务器，转发请求。

        throw new NotImplementedException();
    }
}