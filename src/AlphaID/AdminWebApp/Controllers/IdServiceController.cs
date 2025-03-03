using AdminWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdServiceController(IdApiService apiService) : ControllerBase
{
    [HttpGet("SearchPerson")]
    public async Task<IEnumerable<NaturalPersonModel>> SearchPersonAsync(string term)
    {
        IEnumerable<NaturalPersonModel> result = await apiService.SearchPersonAsync(term);
        return result;
    }

    [HttpGet("SearchOrganization")]
    public async Task<IEnumerable<OrganizationModel>> SearchOrganizationAsync(string term)
    {
        IEnumerable<OrganizationModel> result = await apiService.SearchOrganizationAsync(term);
        return result;
    }
}