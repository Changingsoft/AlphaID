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

        var response = await client.GetAsync("/api/Organization/a7be43af-8b49-450e-a600-90a8748e48a5");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<OrganizationModel>();
        Assert.Equal("蜀汉集团", data!.Name);
    }

    [Fact]
    public async Task GetOrganizationMemberAsync()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/Organization/a7be43af-8b49-450e-a600-90a8748e48a5/Members");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<OrganizationMemberModel[]>();
        Assert.NotEmpty(data!);
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
