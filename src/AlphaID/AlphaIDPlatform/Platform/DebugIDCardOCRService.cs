namespace AlphaIDPlatform.Platform;

/// <summary>
/// Chinese ID Card OCR Service for DEBUG.
/// </summary>
public class DebugIdCardOcrService : IChineseIdCardOcrService
{
    /// <summary>
    /// Recognize ID Card Back.
    /// </summary>
    /// <param name="idCardBackImageData"></param>
    /// <returns></returns>
    public Task<ChineseIdCardBackOcrResult> RecognizeIdCardBack(Stream idCardBackImageData)
    {
        return Task.FromResult(new ChineseIdCardBackOcrResult()
        {
            Issuer = "DebugTestIssuer",
            IssueDate = new DateTime(2020, 1, 1),
        });
    }

    /// <summary>
    /// RecognizeIDCardFront
    /// </summary>
    /// <param name="idCardFrontImageData"></param>
    /// <returns></returns>
    public Task<ChineseIdCardFrontOcrResult> RecognizeIdCardFront(Stream idCardFrontImageData)
    {
        return Task.FromResult(new ChineseIdCardFrontOcrResult()
        {
            Name = "张三",
            SexString = "男",
            Nationality = "汉",
            DateOfBirth = new DateTime(1999, 1, 1),
            Address = "云南省曲靖市麒麟区",
            IdCardNumber = "331023198605055652",
        });

    }
}
