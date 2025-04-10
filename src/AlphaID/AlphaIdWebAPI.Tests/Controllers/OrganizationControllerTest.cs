using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AlphaIdWebAPI.Tests.Controllers;

[Collection(nameof(TestServerCollection))]
public class OrganizationControllerTest(AlphaIdApiFactory factory)
{
    [Fact]
    public async Task GetExistsOrganization()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/Organization/a7be43af-8b49-450e-a600-90a8748e48a5");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<OrganizationModel>();
        Assert.Equal("蜀汉集团", data!.Name);
    }


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

    internal record OrganizationModel(string? Domicile, string? Contact, string? LegalPersonName, DateTime? Expires)
    {
        public string SubjectId { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    internal record OrganizationSearchResult(IEnumerable<OrganizationModel> Organizations, bool More);
}