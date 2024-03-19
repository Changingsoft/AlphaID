using AlphaIdPlatform;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Web;

namespace AdminWebApp.Services;

public class IdApiService(IOptions<SystemUrlInfo> options, IHttpContextAccessor httpContextAccessor)
{
    private readonly SystemUrlInfo options = options.Value;
    private readonly HttpClient client = new();

    public async Task<IEnumerable<NaturalPersonModel>> SearchPersonAsync(string keywords)
    {
        var uri = new Uri(this.options.WebApiUrl, "/api/Person/Suggestions?q={0}");
        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(uri.ToString(), HttpUtility.UrlEncode(keywords)));
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await httpContextAccessor.HttpContext!.GetTokenAsync("access_token") ?? throw new InvalidOperationException("由于找不到AccessToken，对接口的调用失败。"));
        var response = await this.client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<NaturalPersonModel>>() ?? throw new InvalidOperationException("接口没有返回结果。");
    }

    public async Task<IEnumerable<OrganizationModel>> SearchOrganizationAsync(string keywords)
    {
        var uri = new Uri(this.options.WebApiUrl, "/api/Organization/Suggestions?q={0}");
        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(uri.ToString(), HttpUtility.UrlEncode(keywords)));
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await httpContextAccessor.HttpContext!.GetTokenAsync("access_token") ?? throw new InvalidOperationException("由于找不到AccessToken，对接口的调用失败。"));
        var response = await this.client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<OrganizationModel>>() ?? throw new InvalidOperationException("接口没有返回结果。");
    }
}
