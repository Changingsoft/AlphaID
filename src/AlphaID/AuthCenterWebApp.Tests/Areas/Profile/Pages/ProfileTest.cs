namespace AuthCenterWebApp.Tests.Areas.Profile.Pages;

[Collection(nameof(TestServerCollection))]
public class ProfileTest(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task GetDefaultAvatarWhenPersonNotFoundOrProfilePictureNotSet()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("People/unknown-user/Avatar");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/png", response.Content.Headers.ContentType?.ToString());
    }

    [Theory]
    [InlineData("liubei")]
    [InlineData("d2480421-8a15-4292-8e8f-06985a1f645b")]
    public async Task GetAvatar(string anchor)
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        HttpResponseMessage response = await client.GetAsync($"People/{anchor}/Avatar");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/jpeg", response.Content.Headers.ContentType?.ToString());
    }
}