namespace AuthCenterWebAppTests.Areas.Profile.Pages;
public class ProfileTest : IClassFixture<AuthCenterWebAppFactory>
{
    private readonly AuthCenterWebAppFactory factory;

    public ProfileTest(AuthCenterWebAppFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetAvatar()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/Profile/Avatar");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/jpeg", response.Content.Headers.ContentType?.ToString());
    }
}
