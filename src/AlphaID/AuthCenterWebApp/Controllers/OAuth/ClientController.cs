using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenterWebApp.Controllers.OAuth;

/// <summary>
/// OIDC Client.
/// </summary>
/// <param name="dbContext"></param>
[ApiController]
[Route("api/OAuth/Client")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ClientController(ConfigurationDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// 通过client-id获取客户端名称等信息。
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns></returns>
    /// <response code="200">如果找到了客户端，则返回其信息。</response>
    /// <response code="404">没有找到客户端。</response>
    [HttpGet("{clientId}")]
    public ActionResult<ClientInfo> GetClientInfo(string clientId)
    {
        var clients = from client in dbContext.Clients
                      where client.ClientId == clientId
                      select new ClientInfo
                      {
                          Name = client.ClientName,
                          UpdatedAt = client.Updated.HasValue ? client.Updated.Value.ToUniversalTime() : client.Created.ToUniversalTime(),
                      };
        var clientInfo = clients.FirstOrDefault();
        if (clientInfo == null)
            return NotFound();
        return clientInfo;
    }

    /// <summary>
    /// 客户端信息
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// 客户端名称。
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 更新时间。
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    };
}