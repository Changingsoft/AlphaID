namespace AuthCenterWebApp.Tests.Areas.Profile.Pages;

public class ProfileTest(AuthCenterWebAppFactory factory) : IClassFixture<AuthCenterWebAppFactory>
{
    [Fact]
    public async Task GetDefaultPictureWhenPersonNotSpecified()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("People/guanyu/Avatar");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/png", response.Content.Headers.ContentType?.ToString());
    }
}