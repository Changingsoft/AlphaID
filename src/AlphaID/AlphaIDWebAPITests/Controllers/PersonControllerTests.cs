using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AlphaIDWebAPITests.Controllers;

[Collection(nameof(TestServerCollection))]
public class PersonControllerTests
{
    private readonly AlphaIDAPIFactory factory;

    public PersonControllerTests(AlphaIDAPIFactory factory)
    {
        this.factory = factory;
    }


    [Fact(Skip = "暂时跳过")]
    public async Task GetNonExistsPerson()
    {
        var client = this.factory.CreateAuthenticatedClient();
        var personId = "abcdefg";
        var response = await client.GetAsync($"/api/Person/{personId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetExistsPerson()
    {
        var client = this.factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/api/person/d2480421-8a15-4292-8e8f-06985a1f645b");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<PersonModel>();
        Assert.NotNull(data);
    }

    [Fact]
    public async Task GetPersonMembersOf()
    {
        var client = this.factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/api/Person/bf16436b-d15f-44b7-bd61-831eacee5063/MembersOf");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<OrganizationMemberModel[]>();
        Assert.Single(data!);

    }

    [Fact]
    public async Task SearchPersonByPhoneticHint()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/Person/Search/liubei");
        _ = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var data = JsonConvert.DeserializeObject<PersonSearchResult>(await response.Content.ReadAsStringAsync());
        Assert.Single(data!.Persons!);
    }

    [Fact]
    public async Task SearchPersonByMobilePhone()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/Person/Search/13812340001");
        response.EnsureSuccessStatusCode();
        var data = JsonConvert.DeserializeObject<PersonSearchResult>(await response.Content.ReadAsStringAsync());
        Assert.Single(data!.Persons!);
    }
}
