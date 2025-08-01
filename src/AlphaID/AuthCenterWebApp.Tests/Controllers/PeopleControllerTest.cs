namespace AuthCenterWebApp.Tests.Controllers;
public class PeopleControllerTest
{
    [Fact]
    public async Task GetAvatar()
    {
        var factory = new AuthCenterWebAppFactory();
        HttpClient client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/People/liubei/Avatar");
        response.EnsureSuccessStatusCode();
    }
}
