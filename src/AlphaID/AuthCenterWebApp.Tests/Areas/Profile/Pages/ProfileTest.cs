namespace AuthCenterWebApp.Tests.Areas.Profile.Pages;
public class ProfileTest : IClassFixture<AuthCenterWebAppFactory>
{
    private readonly AuthCenterWebAppFactory factory;

    public ProfileTest(AuthCenterWebAppFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetDefaultPictureWhenPersonNotSpecified()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("People/guanyu/Avatar");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/png", response.Content.Headers.ContentType?.ToString());
    }
}
