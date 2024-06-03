using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AlphaIdWebAPI.Tests.Controllers;

[Collection(nameof(TestServerCollection))]
public class PersonControllerTests(AlphaIdApiFactory factory)
{
    [Theory]
    [InlineData("刘备")]
    [InlineData("liubei")]
    public async Task SearchPerson(string keywords)
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response =
            await client.GetAsync($"/api/Person/Suggestions?q={WebUtility.UrlEncode(keywords)}");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<IEnumerable<SearchPersonModel>>();
        Assert.True(data!.Any());
        SearchPersonModel one = data!.First();
        Assert.NotNull(one.AvatarUrl);
    }

    [Fact]
    public async Task GetUserInfo()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/Person/liubei");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<UserInfoModel>();
        Assert.Equal("d2480421-8a15-4292-8e8f-06985a1f645b", data!.SubjectId);
        Assert.Equal("刘备", data.Name);
        Assert.Equal("LIUBEI", data.SearchHint);
        Assert.NotNull(data.AvatarUrl);
    }

    internal record SearchPersonModel(string UserName, string Name, string? AvatarUrl);

    internal record UserInfoModel(string SubjectId, string Name, string? SearchHint, string? AvatarUrl);
}