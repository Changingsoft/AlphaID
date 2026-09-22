namespace AuthCenterWebApp.Tests.Controllers;

[Collection<TestServerCollection>]
public class PeopleControllerTest(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task GetAvatar()
    {
        HttpClient client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/People/liubei/Avatar", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
