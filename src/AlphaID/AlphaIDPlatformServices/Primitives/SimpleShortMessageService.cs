using AlphaIDPlatform.Platform;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AlphaID.PlatformServices.Primitives;

/// <summary>
/// 
/// </summary>
public class SimpleShortMessageService : IVerificationCodeService, IShortMessageService
{
    private readonly HttpClient client;
    private readonly SimpleShortMessageServiceOptions options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settings"></param>
    public SimpleShortMessageService(IOptions<SimpleShortMessageServiceOptions> settings)
    {

        this.options = settings.Value;
        this.client = new HttpClient
        {
            BaseAddress = new Uri(ApiBase)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mobile"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SendAsync(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
        {
            throw new ArgumentException($"“{nameof(mobile)}”不能为 Null 或空白", nameof(mobile));
        }

        var form = new Dictionary<string, string>
        {
            {"recipient", mobile.Trim() }
        };

        await this.AuthenticateAsync(this.client);

        var result = await this.client.PostAsync("/api/VerificationCode", new FormUrlEncodedContent(form));
        if (result.IsSuccessStatusCode)
            return;

        throw new InvalidOperationException($"远程调用发生错误。代码：{result.StatusCode}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<bool> VerifyAsync(string mobile, string code)
    {
        if (string.IsNullOrWhiteSpace(mobile))
        {
            throw new ArgumentException($"“{nameof(mobile)}”不能为 Null 或空白", nameof(mobile));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException($"“{nameof(code)}”不能为 Null 或空白", nameof(code));
        }

        var form = new Dictionary<string, string>
        {
            {"recipient", mobile.Trim() },
            {"code", code.Trim() }
        };

        await this.AuthenticateAsync(this.client);

        var result = await this.client.PostAsync("/api/VerificationCode/Verify", new FormUrlEncodedContent(form));
        return result.IsSuccessStatusCode
            ? bool.Parse(await result.Content.ReadAsStringAsync())
            : throw new InvalidOperationException($"远程调用发生错误。代码：{result.StatusCode}");
    }

    private async Task AuthenticateAsync(HttpClient client)
    {
        try
        {
            await this.EnsureAccessTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
        catch (Exception)
        {
            throw;
        }

    }

    private async Task EnsureAccessTokenAsync()
    {
        if (!AccessTokenExpires.HasValue || AccessTokenExpires.Value < DateTime.UtcNow.AddMinutes(3))
        {
            using var authClient = new HttpClient();
            authClient.BaseAddress = new Uri("https://federal.changingsoft.com");

            var form = new Dictionary<string, string>
            {
                {"client_id",  this.options.ClientId},
                {"client_secret", this.options.ClientSecret },
                {"grant_type", "client_credentials" },
                {"resource", "http://webapi.changingsoft.com/adfs/services/trust" }
            };

            var response = await authClient.PostAsync("/adfs/oauth2/token", new FormUrlEncodedContent(form));

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"远程服务器返回错误{response.StatusCode}");

            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as dynamic;

            AccessToken = (string)result!.access_token;
            AccessTokenExpires = DateTime.UtcNow.AddSeconds((int)result.expires_in);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SendAsync(string mobile, string content)
    {
        if (string.IsNullOrWhiteSpace(mobile))
        {
            throw new ArgumentException($"“{nameof(mobile)}”不能为 Null 或空白", nameof(mobile));
        }


        await this.AuthenticateAsync(this.client);

        var shortMessage = new FreeTextMessage(new[] { mobile }, content);
        var jsonContent = JsonContent.Create(shortMessage);

        var result = await this.client.PostAsync("/api/FreeTextMessage", jsonContent);
        if (result.IsSuccessStatusCode)
            return;

        throw new InvalidOperationException($"远程调用发生错误。代码：{result.StatusCode}");
    }

    private const string ApiBase = "https://sms.changingsoft.com";

    private static string AccessToken { get; set; } = default!;

    private static DateTime? AccessTokenExpires { get; set; }
}
