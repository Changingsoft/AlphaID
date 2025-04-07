using AdminWebApp.Services;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdServiceController(IQueryableUserStore<NaturalPerson> userStore, IOrganizationStore organizationStore, ProfileUrlGenerator<NaturalPerson> generator) : ControllerBase
{
    [HttpGet("SearchPerson")]
    public IEnumerable<NaturalPersonModel> SearchPersonAsync(string term)
    {
        return from person in userStore.Users
               where person.UserName!.StartsWith(term) 
                     || person.Name!.StartsWith(term) 
                     || person.SearchHint!.StartsWith(term)
                     || person.PhoneNumber!.StartsWith(term)
                     || person.Email!.StartsWith(term)
               select new NaturalPersonModel
               {
                   Name = person.Name,
                   UserName = person.UserName!,
                   AvatarUrl = generator.GenerateProfilePictureUrl(person).ToString(),
               };
    }

    [HttpGet("SearchOrganization")]
    public IEnumerable<OrganizationModel> SearchOrganizationAsync(string term)
    {
        return organizationStore.Organizations
            .Where(o => o.Name.Contains(term))
            .Select(o => new OrganizationModel
            {
                SubjectId = o.Id,
                Name = o.Name,
                Domicile = o.Domicile,
                Contact = o.Contact,
            });
    }

    public class OrganizationModel
    {
        public string SubjectId { get; set; } = null!;
        public string Name { get; set; } = null!;

        public string? Domicile { get; set; }

        public string? Contact { get; set; }

        public DateTime? Expires { get; set; }
    }

    public class NaturalPersonModel
    {
        public string UserName { get; set; } = null!;
        public string? Name { get; set; }
        public string? AvatarUrl { get; init; }
    }
}