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
    private readonly ChineseIDCardManager? chineseIDCardManager;

    /// <summary>
    /// Init Person Controller.
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="organizationMemberManager"></param>
    /// <param name="chineseIDCardManager"></param>
    public PersonController(NaturalPersonManager personManager, OrganizationMemberManager organizationMemberManager, ChineseIDCardManager? chineseIDCardManager)
    {
        this.personManager = personManager;
        this.organizationMemberManager = organizationMemberManager;
        this.chineseIDCardManager = chineseIDCardManager;
    }

    /// <summary>
    /// 获取自然人基本信息。
    /// </summary>
    /// <param name="id">自然人的SubjectId</param>
    /// <returns></returns>
    /// <response code="200">返回自然人的基本信息。</response>
    /// <response code="204">没有找到信息。</response>
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
    /// <returns>返回匹配的自然人信息。如果匹配数量超过50条，则只返回前50条。</returns>
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
            var result = this.personManager.Users.SingleOrDefault(p => p.Mobile == number.ToString());
            if (result == null)
                return new PersonSearchResult(Enumerable.Empty<PersonModel>());

            ChineseIDCardValidation? card = null;
            if (this.chineseIDCardManager != null)
                card = await this.chineseIDCardManager.GetCurrentAsync(result);
            return new PersonSearchResult(new PersonModel[] { new PersonModel(result, card != null) });
        }

        var pinyinSearchSet = this.personManager.Users.Where(p => p.PhoneticSearchHint!.StartsWith(keywords)).OrderBy(p => p.PhoneticSearchHint!.Length).ThenBy(p => p.PhoneticSearchHint);
        var pinyinSearchSetCount = pinyinSearchSet.Count();
        var pinyinSearchResult = new List<NaturalPerson>(pinyinSearchSet.Take(30));

        var nameSearchSet = this.personManager.Users.Where(p => p.Name.StartsWith(keywords)).OrderBy(p => p.Name.Length).ThenBy(p => p.Name);
        var nameSearchSetCount = nameSearchSet.Count();
        var nameSearchResult = new List<NaturalPerson>(nameSearchSet.Take(30));

        var searchResults = pinyinSearchResult.UnionBy(nameSearchResult, p => p.Id);

        var final = new List<PersonModel>();
        foreach (var person in searchResults)
        {
            ChineseIDCardValidation? chineseIDCard = null;
            if (this.chineseIDCardManager != null)
                chineseIDCard = await this.chineseIDCardManager.GetCurrentAsync(person);
            final.Add(new PersonModel(person, chineseIDCard != null, members: await this.organizationMemberManager.GetMembersOfAsync(person)));
        }

        return new PersonSearchResult(final, pinyinSearchSetCount > 30 || nameSearchSetCount > 30);
    }

}
