using AlphaIDPlatform;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Web;

namespace AdminWebApp.Services;

public class IdApiService
{
    private readonly SystemUrlOptions options;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly HttpClient client = new();

    public IdApiService(IOptions<SystemUrlOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        this.options = options.Value;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<PersonSearchResult> SearchPersonAsync(string keywords)
    {
        var uri = new Uri(this.options.WebApiUrl, "/api/Person/Search/{0}");
        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(uri.ToString(), HttpUtility.UrlEncode(keywords)));
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await this.httpContextAccessor.HttpContext!.GetTokenAsync("access_token") ?? throw new InvalidOperationException("由于找不到AccessToken，对接口的调用失败。"));
        var response = await this.client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PersonSearchResult>() ?? throw new InvalidOperationException("接口没有返回结果。");
    }

    public async Task<OrganizationSearchResult> SearchOrganizationAsync(string keywords)
    {
        var uri = new Uri(this.options.WebApiUrl, "/api/Organization/Search/{0}");
        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(uri.ToString(), HttpUtility.UrlEncode(keywords)));
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await this.httpContextAccessor.HttpContext!.GetTokenAsync("access_token") ?? throw new InvalidOperationException("由于找不到AccessToken，对接口的调用失败。"));
        var response = await this.client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrganizationSearchResult>() ?? throw new InvalidOperationException("接口没有返回结果。");
    }
}
