namespace AlphaIDPlatform.Platform;

/// <summary>
/// Chinese ID Card OCR Service for DEBUG.
/// </summary>
public class DebugIDCardOCRService : IChineseIDCardOCRService
{
    /// <summary>
    /// Recognize ID Card Back.
    /// </summary>
    /// <param name="idCardBackImageData"></param>
    /// <returns></returns>
    public Task<ChineseIDCardBackOCRResult> RecognizeIDCardBack(Stream idCardBackImageData)
    {
        return Task.FromResult(new ChineseIDCardBackOCRResult()
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
    public Task<ChineseIDCardFrontOCRResult> RecognizeIDCardFront(Stream idCardFrontImageData)
    {
        return Task.FromResult(new ChineseIDCardFrontOCRResult()
        {
            Name = "张三",
            SexString = "男",
            Nationality = "汉",
            DateOfBirth = new DateTime(1999, 1, 1),
            Address = "云南省曲靖市麒麟区",
            IDCardNumber = "331023198605055652",
        });

    }
}
