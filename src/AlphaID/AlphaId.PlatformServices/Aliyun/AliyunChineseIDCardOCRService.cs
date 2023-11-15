using AlphaIdPlatform.Platform;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AlphaId.PlatformServices.Aliyun;

/// <summary>
/// 阿里云身份证识别服务。
/// </summary>
public class AliyunChineseIdCardOcrService : IChineseIdCardOcrService
{
    private readonly AliyunChineseIdCardOcrServiceOptions options;

    /// <summary>
    /// 
    /// </summary>
    public AliyunChineseIdCardOcrService(IOptions<AliyunChineseIdCardOcrServiceOptions> options)
    {
        this.options = options.Value;
    }
    /// <summary>
    /// 识别身份证背面（国徽面）。
    /// </summary>
    /// <param name="idCardBackImageData"></param>
    /// <returns></returns>
    public async Task<ChineseIdCardBackOcrResult> RecognizeIdCardBack(Stream idCardBackImageData)
    {
        string imgBase64;
        using (var ms = new MemoryStream())
        {
            await idCardBackImageData.CopyToAsync(ms);
            imgBase64 = Convert.ToBase64String(ms.ToArray());
        }
        var requestData = new
        {
            image = imgBase64,
            configure = new
            {
                side = "back",
            },
        };

        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(this.options.ServiceBaseUrl),
        };

        var requestMsg = new HttpRequestMessage(HttpMethod.Post, "/rest/160601/ocr/ocr_idcard.json");
        requestMsg.Headers.Authorization = new AuthenticationHeaderValue("APPCODE", this.options.AppCode);
        requestMsg.Content = JsonContent.Create(requestData);
        //requestMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json; charset=UTF-8");

        var responseMsg = await httpClient.SendAsync(requestMsg);
        responseMsg.EnsureSuccessStatusCode();
        var result = JsonConvert.DeserializeObject<dynamic>(await responseMsg.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException("无法从响应取得数据");

        if (!(bool)result.success)
            throw new ChineseIdCardOcrException("Can not recognize");
        var returnResult = new ChineseIdCardBackOcrResult
        {
            Issuer = (string)result.issue,
            IssueDate = ParseDate((string)result.start_date),
            ExpiresDate = ParseDate((string)result.end_date),
        };
        return returnResult;
    }


    /// <summary>
    /// 识别身份证正面（个人信息面）。
    /// </summary>
    /// <param name="idCardFrontImageData"></param>
    /// <returns></returns>
    public async Task<ChineseIdCardFrontOcrResult> RecognizeIdCardFront(Stream idCardFrontImageData)
    {
        string imgBase64;
        using (var ms = new MemoryStream())
        {
            await idCardFrontImageData.CopyToAsync(ms);
            imgBase64 = Convert.ToBase64String(ms.ToArray());
        }
        var requestData = new
        {
            image = imgBase64,
            configure = new
            {
                side = "face",
            },
        };
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(this.options.ServiceBaseUrl),
        };

        var requestMsg = new HttpRequestMessage(HttpMethod.Post, "/rest/160601/ocr/ocr_idcard.json");
        requestMsg.Headers.Authorization = new AuthenticationHeaderValue("APPCODE", this.options.AppCode);
        requestMsg.Content = JsonContent.Create(requestData);
        //requestMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json; charset=UTF-8");

        var responseMsg = await httpClient.SendAsync(requestMsg);
        responseMsg.EnsureSuccessStatusCode();
        var result = JsonConvert.DeserializeObject<dynamic>(await responseMsg.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException("无法从响应中取得数据。");

        if (!(bool)result.success)
            throw new ChineseIdCardOcrException("Can not recognize");
        var returnResult = new ChineseIdCardFrontOcrResult
        {
            Name = (string)result.name,
            SexString = (string)result.sex,
            Nationality = (string)result.nationality,
            DateOfBirth = ParseDate((string)result.birth),
            Address = (string)result.address,
            IdCardNumber = (string)result.num,
        };

        return returnResult;

    }

    private static DateTime ParseDate(string dateString)
    {
        var year = int.Parse(dateString[..4]);
        var month = int.Parse(dateString.Substring(4, 2));
        var day = int.Parse(dateString.Substring(6, 2));
        return new DateTime(year, month, day);
    }
}
