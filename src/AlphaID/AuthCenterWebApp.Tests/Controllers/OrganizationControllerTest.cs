using System.Net;
using System.Net.Http.Json;

namespace AuthCenterWebApp.Tests.Controllers;

public class OrganizationControllerTest
{
    /// <summary>
    /// 使用OrganizationId查询一个已存在的组织。
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetExistsOrganization()
    {
        var webFactory = new AuthCenterWebAppFactory();
        HttpClient client = webFactory.CreateBearerTokenClient();
        HttpResponseMessage response = await client.GetAsync("/api/Organization/a7be43af-8b49-450e-a600-90a8748e48a5");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<OrganizationModel>();
        Assert.True(Uri.TryCreate(data!.ProfileUrl, UriKind.Absolute, out _));
        Assert.True(Uri.TryCreate(data.ProfilePictureUrl, UriKind.Absolute, out _));
    }

    [Fact]
    public async Task SearchOrganizations()
    {
        var webFactory = new AuthCenterWebAppFactory();
        HttpClient client = webFactory.CreateBearerTokenClient();
        HttpResponseMessage response =
            await client.GetAsync($"/api/Organization/Suggestions?q={WebUtility.UrlEncode("蜀汉")}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<OrganizationSearchModel>>();
        var firstItem = result!.First();
        Assert.True(Uri.TryCreate(firstItem.ProfilePictureUrl, UriKind.Absolute, out _));
    }

    /// <summary>
    /// 测试被禁用的组织排除在搜索结果之外。
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SearchWillExcludeDisabledOrgs()
    {
        var webFactory = new AuthCenterWebAppFactory();
        HttpClient client = webFactory.CreateBearerTokenClient();
        HttpResponseMessage response =
            await client.GetAsync($"/api/Organization/Suggestions?q={WebUtility.UrlEncode("改名后的有限公司")}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<IEnumerable<OrganizationSearchModel>>();
        Assert.Empty(json!);
    }

    [Fact]
    public async Task OrganizationSuggestionsRateLimitTest()
    {
        var webFactory = new AuthCenterWebAppFactory();
        var client = webFactory.CreateClient();
        HttpResponseMessage response = null!;
        for (var i = 0; i < 300; i++)
        {
            response = await client.GetAsync("/api/Organization/Suggestions?q=测试");
        }
        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    [Fact]
    public async Task GetProfilePicture()
    {
        var factory = new AuthCenterWebAppFactory();
        HttpClient client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/Organization/蜀汉集团/Picture");
        response.EnsureSuccessStatusCode();
        var contentType = response.Content.Headers.ContentType?.MediaType;
        Assert.Equal("image/png", contentType);
    }

    internal record OrganizationModel
    {
        public string Name { get; set; } = null!;

        public string? Domicile { get; set; }

        public string? Representative { get; set; }

        public string? ProfileUrl { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? LocationWkt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    internal class OrganizationSearchModel
    {
        public string Subject { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Domicile { get; set; }

        public string? Representative { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}