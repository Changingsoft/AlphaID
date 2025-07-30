using System.Net;
using System.Net.Http.Json;

namespace AuthCenterWebApp.Tests.Controllers;

[Collection(nameof(TestServerCollection))]
public class OrganizationControllerTest(AuthCenterWebAppFactory factory)
{
    /// <summary>
    /// 使用OrganizationId查询一个已存在的组织。
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetExistsOrganization()
    {
        HttpClient client = factory.CreateBearerTokenClient();

        HttpResponseMessage response = await client.GetAsync("/api/Organization/a7be43af-8b49-450e-a600-90a8748e48a5");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<OrganizationModel>();
        Assert.Equal("蜀汉集团", data!.Name);
    }

    /// <summary>
    /// 测试被禁用的组织排除在搜索结果之外。
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SearchWillExcludeDisabledOrgs()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        HttpResponseMessage response =
            await client.GetAsync($"/api/Organization/Suggestions?q={WebUtility.UrlEncode("改名后的有限公司")}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<IEnumerable<OrganizationModel>>();
        Assert.Empty(json!);
    }

    [Fact]
    public async Task OrganizationSuggestionsRateLimitTest()
    {
        var client = factory.CreateClient();
        HttpResponseMessage response = null!;
        for (int i = 0; i < 300; i++)
        {
            response = await client.GetAsync("/api/Organization/Suggestions?q=测试");
        }
        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    internal record OrganizationModel(string? Domicile, string? Contact, string? LegalPersonName, DateTime? Expires)
    {
        public string SubjectId { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}