using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace NgAdminWebApp.Controllers;

public class OidcConfigurationController : Controller
{
    public OidcConfigurationController(
        IClientRequestParametersProvider clientRequestParametersProvider)
    {
        this.ClientRequestParametersProvider = clientRequestParametersProvider;
    }

    public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

    [HttpGet("_configuration/{clientId}")]
    public IActionResult GetClientRequestParameters([FromRoute] string clientId)
    {
        var parameters = this.ClientRequestParametersProvider.GetClientParameters(this.HttpContext, clientId);
        return this.Ok(parameters);
    }
}