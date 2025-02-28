using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIdWebAPI.Controllers.OAuth;

/// <summary>
/// OIDC Client.
/// </summary>
/// <param name="dbContext"></param>
[ApiController]
[Route("api/OAuth/Client")]
[Authorize]
public class ClientController(ConfigurationDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// 通过 client-id 获取客户端信息。
    /// </summary>
    /// <remarks>
    /// 任何已经身份验证的依赖方，可通过此接口获取指定client-id的客户端的信息。
    /// </remarks>
    /// <param name="clientId">Client ID</param>
    /// <returns></returns>
    /// <response code="200">如果找到了客户端，则返回其信息。</response>
    /// <response code="404">没有找到客户端。</response>
    [HttpGet("{clientId}")]
    public ActionResult<ClientInfo> GetClientInfo(string clientId)
    {
        Client? client = dbContext.Clients.FirstOrDefault(p => p.ClientId == clientId);
        return client == null ? NotFound() : new ClientInfo(client.ClientName);
    }

    /// <summary>
    /// 客户端信息
    /// </summary>
    /// <param name="Name">客户端名称</param>
    public record ClientInfo(string Name);
}