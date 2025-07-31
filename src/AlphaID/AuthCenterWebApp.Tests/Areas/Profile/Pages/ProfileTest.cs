namespace AuthCenterWebApp.Tests.Areas.Profile.Pages;

[Collection(nameof(TestServerCollection))]
public class ProfileTest(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task GetDefaultAvatarWhenPersonNotFoundOrProfilePictureNotSet()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("People/unknown-user/Avatar");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/png", response.Content.Headers.ContentType?.ToString());
    }

    [Theory]
    [InlineData("liubei")]
    public async Task GetAvatar(string anchor)
    {
        HttpClient client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync($"People/{anchor}/Avatar");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/jpeg", response.Content.Headers.ContentType?.ToString());
    }
}