using AdminWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdServiceController : ControllerBase
{
    private readonly IdApiService personSearcher;

    public IdServiceController(IdApiService personSearcher)
    {
        this.personSearcher = personSearcher;
    }

    [HttpGet("SearchPerson")]
    public async Task<IEnumerable<NaturalPersonModel>> SearchPersonAsync(string term)
    {
        var result = await this.personSearcher.SearchPersonAsync(term);
        return result.Persons;
    }

    [HttpGet("SearchOrganization")]
    public async Task<IEnumerable<OrganizationModel>> SearchOrganizationAsync(string term)
    {
        var result = await this.personSearcher.SearchOrganizationAsync(term);
        return result.Organizations;
    }
}
