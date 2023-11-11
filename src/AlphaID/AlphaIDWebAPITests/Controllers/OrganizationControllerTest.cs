using AlphaIDWebAPITests.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AlphaIDWebAPITests.Controllers;

[Collection(nameof(TestServerCollection))]
public class OrganizationControllerTest
{
    private readonly AlphaIdApiFactory factory;

    public OrganizationControllerTest(AlphaIdApiFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetExistsOrganization()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/Organization/1c86b543-0c92-4cd8-bcd5-b4e462847e59");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<OrganizationModel>();
        Assert.Equal("子虚乌有公司", data!.Name);
    }

    [Fact]
    public async Task GetOrganizationMemberAsync()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/Organization/5288b813-e1f4-4fd3-a342-6f21a4c3fef7/Members");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<OrganizationMemberModel[]>();
        Assert.Single(data!);
    }

    [Fact]
    public async Task SearchWillExcludeDisabledOrgs()
    {
        var client = this.factory.CreateAuthenticatedClient();
        var response = await client.GetAsync($"/api/Organization/Search/{WebUtility.UrlEncode("改名后的有限公司")}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<OrganizationSearchResult>();
        Assert.DoesNotContain(json!.Organizations, p => p.Name.Contains("改名后的有限公司"));
    }
}
