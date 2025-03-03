using System.Diagnostics;
using AlphaIdPlatform.Platform;
using Newtonsoft.Json;

namespace AlphaId.PlatformServices.Tianyancha;

/// <summary>
/// 天眼查工商信息服务。
/// </summary>
public class TianYanChaOrganizationInfoProvider : IOrganizationInfoProvider
{
    private const string Url = "http://open.api.tianyancha.com/services/open/ic/baseinfo/2.0";
    private const string Token = "1e439145-9eec-48d9-b22c-99d7fda0cb92";
    private readonly JsonSerializer _serializer;

    /// <summary>
    /// </summary>
    public TianYanChaOrganizationInfoProvider()
    {
        _serializer = new JsonSerializer();
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<OrganizationInfo?> FindAsync(string name)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(Url)
        };

        client.DefaultRequestHeaders.Add("Authorization", Token);


        HttpResponseMessage response = await client.GetAsync("");

        var jsonReader = new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync()));

        var result = _serializer.Deserialize(jsonReader) as dynamic;

        if ((int)result!.error_code != 0)
        {
            Trace.TraceWarning($"天眼查返回错误。代码：{result.error_code}，消息：{result.reson}");
            return null;
        }

        return new OrganizationInfo(result.creditCode, result.name, result.regLocation, string.Empty, string.Empty,
            result.orgNumber, result.taxNumber);
    }
}