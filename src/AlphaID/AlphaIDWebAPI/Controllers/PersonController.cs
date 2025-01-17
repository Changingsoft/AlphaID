using AlphaIdPlatform;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Security;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIdWebAPI.Controllers;

/// <summary>
///     与自然人有关的查询。
/// </summary>
/// <remarks>
///     Init Person Controller.
/// </remarks>
/// <param name="personManager"></param>
/// <param name="urlInfo"></param>
/// <param name="memberManager"></param>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PersonController(
    UserManager<NaturalPerson> personManager,
    IOptions<SystemUrlInfo> urlInfo,
    OrganizationMemberManager memberManager) : ControllerBase
{
    private readonly SystemUrlInfo _urlInfo = urlInfo.Value;

    /// <summary>
    ///     通过 UserName 获取指定用户的信息。
    /// </summary>
    /// <remarks>
    /// 重要提示！为保持兼容性，该查询接口仍支持传入自然人的SubjectId，但在后续版本中将不再支持。
    /// </remarks>
    /// <param name="userName">用户名</param>
    /// <returns></returns>
    [HttpGet("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonInfoModel>> GetUserInfoAsync(string userName)
    {
        //hack 为保持兼容性，下一版本将不再支持SubjectId查询
        NaturalPerson? person = await personManager.FindByNameAsync(userName) ?? await personManager.FindByIdAsync(userName);
        if (person == null)
            return NotFound();

        return new PersonInfoModel(person.Id, person.HumanName?.FullName, person.SearchHint,
            new Uri(_urlInfo.AuthCenterUrl, $"/People/{person.Id}/Avatar").ToString());
    }


    /// <summary>
    ///     查找某个自然人。
    /// </summary>
    /// <param name="q">关键词。关键词非空长度必须大于2个字符时，才会返回可用结果。可以通过用户名、全名来查找自然人。</param>
    /// <returns>Return matched peoples, up to 50 records.</returns>
    [HttpGet("Suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IEnumerable<SuggestedPersonModel> SearchAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return [];

        q = q.Trim();
        if (q.Length < 2)
            return [];

        HashSet<SuggestedPersonModel> set = [];

        if (q.Length >= 3)
        {
            IQueryable<SuggestedPersonModel> pinyinSearchSet = personManager.Users
                .Where(p => p.SearchHint!.StartsWith(q))
                .OrderBy(p => p.SearchHint!.Length)
                .ThenBy(p => p.SearchHint)
                .Take(10).Select(p => new SuggestedPersonModel(p)
                {
                    AvatarUrl = new Uri(_urlInfo.AuthCenterUrl, $"/People/{p.Id}/Avatar").ToString()
                });
            set.UnionWith(pinyinSearchSet);
        }

        if (q.Length >= 4)
        {
            IQueryable<SuggestedPersonModel> userNameSearchSet = personManager.Users
                .Where(p => p.UserName!.StartsWith(q))
                .OrderBy(p => p.UserName!.Length)
                .ThenBy(p => p.UserName)
                .Take(10).Select(p => new SuggestedPersonModel(p)
                {
                    AvatarUrl = new Uri(_urlInfo.AuthCenterUrl, $"/People/{p.Id}/Avatar").ToString()
                });
            set.UnionWith(userNameSearchSet);
        }

        IQueryable<SuggestedPersonModel> nameSearchSet = personManager.Users
            .Where(p => p.HumanName!.FullName.StartsWith(q))
            .OrderBy(p => p.HumanName!.FullName.Length)
            .ThenBy(p => p.HumanName!.FullName)
            .Take(10).Select(p => new SuggestedPersonModel(p)
            {
                AvatarUrl = new Uri(_urlInfo.AuthCenterUrl, $"/People/{p.Id}/Avatar").ToString()
            });
        set.UnionWith(nameSearchSet);


        return set;
    }

    /// <summary>
    /// 查找某个自然人。
    /// </summary>
    /// <remarks>
    /// 此接口将被废弃。应使用/api/Person/Suggestions。
    /// </remarks>
    /// <param name="keywords">关键词。可以通过手机号码、姓名汉字、姓名全拼</param>
    /// <returns>返回匹配的自然人信息。如果匹配数量超过50条，则只返回前50条。</returns>
    [Obsolete("不再支持，应使用Suggestions。")]
    [HttpGet("Search/{keywords}")]
    public Task<PersonSearchResult> Search(string keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords))
        {
            return Task.FromResult(new PersonSearchResult([]));
        }

        keywords = keywords.Trim();
        if (keywords.Length < 2)
            return Task.FromResult(new PersonSearchResult([]));

        if (MobilePhoneNumber.TryParse(keywords, out MobilePhoneNumber number))
        {
            var result = personManager.Users.SingleOrDefault(p => p.PhoneNumber == number.ToString());
            if (result == null)
                return Task.FromResult(new PersonSearchResult([]));

            return Task.FromResult(new PersonSearchResult([new PersonInfoModel(result.Id, result.HumanName?.FullName, result.SearchHint, new Uri(_urlInfo.AuthCenterUrl, $"/People/{result.Id}/Avatar").ToString())]));
        }

        var pinyinSearchSet = personManager.Users.Where(p => p.SearchHint!.StartsWith(keywords)).OrderBy(p => p.SearchHint!.Length).ThenBy(p => p.SearchHint);
        int pinyinSearchSetCount = pinyinSearchSet.Count();
        var pinyinSearchResult = new List<NaturalPerson>(pinyinSearchSet.Take(30));

        var nameSearchSet = personManager.Users.Where(p => p.HumanName!.FullName.StartsWith(keywords)).OrderBy(p => p.HumanName!.FullName.Length).ThenBy(p => p.HumanName!.FullName);
        int nameSearchSetCount = nameSearchSet.Count();
        var nameSearchResult = new List<NaturalPerson>(nameSearchSet.Take(30));

        var searchResults = pinyinSearchResult.UnionBy(nameSearchResult, p => p.Id);

        var final = new List<PersonInfoModel>();
        foreach (var person in searchResults)
        {
            final.Add(new PersonInfoModel(person.Id, person.HumanName?.FullName, person.SearchHint, new Uri(_urlInfo.AuthCenterUrl, $"/People/{person.Id}/Avatar").ToString()));
        }

        return Task.FromResult(new PersonSearchResult(final, pinyinSearchSetCount > 30 || nameSearchSetCount > 30));
    }

    /// <summary>
    ///     获取指定用户的组织成员身份。
    /// </summary>
    /// <remarks>
    /// 重要提示！MembersOf 接口将被废弃。请使用 Memberships。目前仍支持通过指定SubjectId查找组成员身份，后续将不再支持。
    /// </remarks>
    /// <param name="userName">用户名。</param>
    /// <returns></returns>
    [HttpGet("{userName}/MembersOf")]
    [HttpGet("{userName}/Memberships")]
    public async Task<ActionResult<IEnumerable<MembershipModel>>> GetMemberships(string userName)
    {
        NaturalPerson? visitor = null;
        string? visitorSubjectId = User.SubjectId();
        if (visitorSubjectId != null)
            visitor = await personManager.FindByIdAsync(User.SubjectId()!);

        //hack 为保持兼容性，下一版本将不再支持SubjectId查询
        NaturalPerson? person = await personManager.FindByNameAsync(userName) ?? await personManager.FindByIdAsync(userName);
        if (person == null)
            return new ActionResult<IEnumerable<MembershipModel>>([]);

        IEnumerable<OrganizationMember> members = memberManager.GetVisibleMembersOf(person, visitor);
        return new ActionResult<IEnumerable<MembershipModel>>(members.Select(m => new MembershipModel(m)));
    }

    /// <summary>
    ///     用户信息。
    /// </summary>
    /// <param name="SubjectId">Subject Id</param>
    /// <param name="Name">全名</param>
    /// <param name="SearchHint">搜索提示</param>
    /// <param name="AvatarUrl">头像Url</param>
    public record PersonInfoModel(
        string SubjectId,
        string? Name,
        string? SearchHint,
        string? AvatarUrl)
    {
        /// <summary>
        /// 已废弃，将始终返回false.
        /// </summary>
        [Obsolete("不再使用此属性。")]
        public bool RealNameValid { get; set; } = false;
    };

    /// <summary>
    ///     自然人
    /// </summary>
    /// <param name="UserName">用户名</param>
    /// <param name="Name">全名</param>
    /// <param name="AvatarUrl"></param>
    public record SuggestedPersonModel(
        string UserName,
        string? Name,
        string? AvatarUrl = null)
    {
        /// <summary>
        ///     通过NaturalPerson初始化自然人。
        /// </summary>
        /// <param name="person"></param>
        public SuggestedPersonModel(NaturalPerson person)
            : this(person.UserName!,
                person.HumanName?.FullName)
        {
        }
    }

    /// <summary>
    ///     组织的成员。
    /// </summary>
    /// <param name="Department">部门</param>
    /// <param name="Title">职务</param>
    /// <param name="OrganizationId">组织标识符</param>
    /// <param name="OrganizationName">组织名称</param>
    /// <param name="Remark">备注</param>
    public record MembershipModel(
        string OrganizationId,
        string OrganizationName,
        string? Title,
        string? Department,
        string? Remark)
    {
        /// <summary>
        ///     Init.
        /// </summary>
        /// <param name="member"></param>
        public MembershipModel(OrganizationMember member)
            : this(member.OrganizationId,
                member.Organization.Name,
                member.Title,
                member.Department,
                member.Remark)
        {
        }
    }

    /// <summary>
    /// 自然人搜索结果。
    /// </summary>
    /// <param name="Persons">自然人搜索的结果。</param>
    /// <param name="More">指示一个值，表示该结果不完全，需要尝试更多的关键字来缩小搜索范围。</param>
    [Obsolete("不再使用此结构。")]
    public record PersonSearchResult(IEnumerable<PersonInfoModel> Persons, bool More = false);
}