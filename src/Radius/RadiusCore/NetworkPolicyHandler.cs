
namespace RadiusCore;

internal class NetworkPolicyHandler(ResponseGenerator responseGenerator)
{
    internal async Task HandleAsync(RadiusContext radiusContext)
    {
        //todo：检查连接请求中是否设置了覆盖身份验证方式。

        //验证用户和其提供的机密信息。如果验证通过，发送Access-Accept响应。
        bool condition = true;
        RadiusResponse response = null;
        if (condition)
        {
            response = await responseGenerator.GenerateAcceptResponse(radiusContext.Request);
        }
        else
        {
            response = await responseGenerator.GenerateRejectResponse(radiusContext.Request);
        }

        await radiusContext.Server.SendAsync(response);

        throw new NotImplementedException();
    }
}