using AdminWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdServiceController(IdApiService personSearcher) : ControllerBase
{
    [HttpGet("SearchPerson")]
    public async Task<IEnumerable<NaturalPersonModel>> SearchPersonAsync(string term)
    {
        var result = await personSearcher.SearchPersonAsync(term);
        return result;
    }

    [HttpGet("SearchOrganization")]
    public async Task<IEnumerable<OrganizationModel>> SearchOrganizationAsync(string term)
    {
        var result = await personSearcher.SearchOrganizationAsync(term);
        return result;
    }
}
