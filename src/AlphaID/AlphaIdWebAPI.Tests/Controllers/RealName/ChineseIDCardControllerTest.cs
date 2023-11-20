using System.Net;
using Xunit;

namespace AlphaIdWebAPI.Tests.Controllers.RealName;

[Collection(nameof(TestServerCollection))]
public class ChineseIdCardControllerTest
{
    private readonly AlphaIdApiFactory webApiFactory;

    public ChineseIdCardControllerTest(AlphaIdApiFactory webApiFactory)
    {
        this.webApiFactory = webApiFactory;
    }

    [Fact(Skip = "暂无测试条件。")]
    public async Task GetNonExistsChineseIdCard()
    {
        using var client = this.webApiFactory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/RealName/ChineseIDCard/12345");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(Skip = "暂无测试条件。")]
    public async Task GetExistsChineseIdCard()
    {
        using var client = this.webApiFactory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/RealName/ChineseIDCard/d2480421-8a15-4292-8e8f-06985a1f645b");
        //404
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
