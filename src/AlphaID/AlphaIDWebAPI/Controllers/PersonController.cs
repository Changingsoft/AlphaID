using AlphaIDWebAPI.Models;
using IDSubjects;
using IDSubjects.RealName;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIDWebAPI.Controllers;

/// <summary>
/// 与自然人有关的查询。
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PersonController : ControllerBase
{
    private readonly NaturalPersonManager personManager;
    private readonly OrganizationMemberManager organizationMemberManager;
    private readonly ChineseIdCardManager? chineseIdCardManager;

    /// <summary>
    /// Init Person Controller.
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="organizationMemberManager"></param>
    /// <param name="chineseIdCardManager"></param>
    public PersonController(NaturalPersonManager personManager, OrganizationMemberManager organizationMemberManager, ChineseIdCardManager? chineseIdCardManager)
    {
        this.personManager = personManager;
        this.organizationMemberManager = organizationMemberManager;
        this.chineseIdCardManager = chineseIdCardManager;
    }

    /// <summary>
    /// Get people base information by ID.
    /// </summary>
    /// <param name="id">Subject ID</param>
    /// <returns></returns>
    /// <response code="200">Base info of person.</response>
    /// <response code="204">Person not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonModel?>> GetAsync(string id)
    {
        var person = await this.personManager.FindByIdAsync(id);
        if (person == null)
        {
            return this.NotFound();
        }
        var members = await this.organizationMemberManager.GetMembersOfAsync(person);
        return new PersonModel(person, members: members);
    }

    /// <summary>
    /// 获取自然人所隶属的组织。
    /// </summary>
    /// <param name="id">自然人的SubjectId</param>
    /// <returns></returns>
    [HttpGet("{id}/MembersOf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<OrganizationMemberModel>> MemberOfAsync(string id)
    {
        var person = await this.personManager.FindByIdAsync(id);
        if (person == null)
            return Enumerable.Empty<OrganizationMemberModel>();

        var members = await this.organizationMemberManager.GetMembersOfAsync(person);
        return from member in members select new OrganizationMemberModel(member);
    }

    /// <summary>
    /// 查找某个自然人。
    /// </summary>
    /// <param name="keywords">关键词。可以通过手机号码、姓名汉字、姓名全拼</param>
    /// <returns>Return matched peoples, up to 50 records.</returns>
    [HttpGet("Search/{keywords}")]
    public async Task<PersonSearchResult> Search(string keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords))
        {
            return new PersonSearchResult(Enumerable.Empty<PersonModel>());
        }

        keywords = keywords.Trim();
        if (keywords.Length < 2)
            return new PersonSearchResult(Enumerable.Empty<PersonModel>());

        if (MobilePhoneNumber.TryParse(keywords, out MobilePhoneNumber number))
        {
            var result = this.personManager.Users.SingleOrDefault(p => p.PhoneNumber == number.ToString());
            if (result == null)
                return new PersonSearchResult(Enumerable.Empty<PersonModel>());

            ChineseIdCardValidation? card = null;
            if (this.chineseIdCardManager != null)
                card = await this.chineseIdCardManager.GetCurrentAsync(result);
            return new PersonSearchResult(new PersonModel[] { new(result, card != null) });
        }

        var pinyinSearchSet = this.personManager.Users.Where(p => p.PersonName.SearchHint!.StartsWith(keywords)).OrderBy(p => p.PersonName.SearchHint!.Length).ThenBy(p => p.PersonName.SearchHint);
        var pinyinSearchSetCount = pinyinSearchSet.Count();
        var pinyinSearchResult = new List<NaturalPerson>(pinyinSearchSet.Take(30));

        var nameSearchSet = this.personManager.Users.Where(p => p.PersonName.FullName.StartsWith(keywords)).OrderBy(p => p.PersonName.FullName.Length).ThenBy(p => p.PersonName.FullName);
        var nameSearchSetCount = nameSearchSet.Count();
        var nameSearchResult = new List<NaturalPerson>(nameSearchSet.Take(30));

        var searchResults = pinyinSearchResult.UnionBy(nameSearchResult, p => p.Id);

        var final = new List<PersonModel>();
        foreach (var person in searchResults)
        {
            ChineseIdCardValidation? chineseIdCard = null;
            if (this.chineseIdCardManager != null)
                chineseIdCard = await this.chineseIdCardManager.GetCurrentAsync(person);
            final.Add(new PersonModel(person, chineseIdCard != null, members: await this.organizationMemberManager.GetMembersOfAsync(person)));
        }

        return new PersonSearchResult(final, pinyinSearchSetCount > 30 || nameSearchSetCount > 30);
    }

}
