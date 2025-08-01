using System.Net.Http.Json;

namespace AuthCenterWebApp.Tests.Controllers;
public class MembershipControllerTest
{
    [Fact]
    public async Task GetMembership()
    {
        var factory = new AuthCenterWebAppFactory();
        var client = factory.CreateBearerTokenClient();

        var response = await client.GetAsync("/api/membership");
        response.EnsureSuccessStatusCode();
        var dataResult = await response.Content.ReadFromJsonAsync<IEnumerable<MembershipModel>>();
        var membership = dataResult!.First();
        Assert.NotNull(membership.OrganizationName);
    }

    public class MembershipModel
    {
        public string? Title { get; set; }

        public string? Department { get; set; }

        public string OrganizationName { get; set; } = null!;

        public string OrganizationId { get; set; } = null!;
    }
}
