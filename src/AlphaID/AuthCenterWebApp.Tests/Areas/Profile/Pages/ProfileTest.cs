namespace AuthCenterWebApp.Tests.Areas.Profile.Pages;

[Collection(nameof(TestServerCollection))]
public class ProfileTest(AuthCenterWebAppFactory factory)
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