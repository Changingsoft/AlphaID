using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIDWebAPI.Controllers.Oidc;

/// <summary>
/// OIDC Client.
/// </summary>
[ApiController]
[Route("api/Oidc/Client")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly ConfigurationDbContext dbContext;

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="dbContext"></param>
    public ClientController(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Gets client name by Client Id.
    /// </summary>
    /// <param name="clientId">Client Id.</param>
    /// <returns>Client name when exists, otherwise 404 not found.</returns>
    [HttpGet("{clientId}")]
    public ActionResult<string> GetClientName(string clientId)
    {
        var client = this.dbContext.Clients.FirstOrDefault(p => p.ClientId == clientId);
        return client == null ? (ActionResult<string>)this.NotFound() : (ActionResult<string>)client.ClientName;
    }
}
