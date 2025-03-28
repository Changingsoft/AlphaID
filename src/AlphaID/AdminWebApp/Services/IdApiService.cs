﻿using System.Net.Http.Headers;
using System.Web;
using AlphaIdPlatform;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AdminWebApp.Services;

public class IdApiService(IOptions<SystemUrlInfo> options, IHttpContextAccessor httpContextAccessor)
{
    private readonly HttpClient _client = new();
    private readonly SystemUrlInfo _options = options.Value;

    public async Task<IEnumerable<NaturalPersonModel>> SearchPersonAsync(string keywords)
    {
        var uri = new Uri(_options.WebApiUrl, "/api/Person/Suggestions?q={0}");
        var request = new HttpRequestMessage(HttpMethod.Get,
            string.Format(uri.ToString(), HttpUtility.UrlEncode(keywords)));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
            await httpContextAccessor.HttpContext!.GetTokenAsync("access_token") ??
            throw new InvalidOperationException("由于找不到AccessToken，对接口的调用失败。"));
        HttpResponseMessage response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<NaturalPersonModel>>() ??
               throw new InvalidOperationException("接口没有返回结果。");
    }

    public async Task<IEnumerable<OrganizationModel>> SearchOrganizationAsync(string keywords)
    {
        var uri = new Uri(_options.WebApiUrl, "/api/Organization/Suggestions?q={0}");
        var request = new HttpRequestMessage(HttpMethod.Get,
            string.Format(uri.ToString(), HttpUtility.UrlEncode(keywords)));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
            await httpContextAccessor.HttpContext!.GetTokenAsync("access_token") ??
            throw new InvalidOperationException("由于找不到AccessToken，对接口的调用失败。"));
        HttpResponseMessage response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<OrganizationModel>>() ??
               throw new InvalidOperationException("接口没有返回结果。");
    }
}