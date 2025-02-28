using AlphaIdPlatform.Platform;

namespace AlphaIdPlatform.Debugging;

/// <summary>
/// Chinese ID Card OCR Service for DEBUG.
/// </summary>
public class DebugChineseIdCardOcrService : IChineseIdCardOcrService
{
    /// <summary>
    /// Recognize ID Card Back.
    /// </summary>
    /// <param name="idCardBackImageData"></param>
    /// <returns></returns>
    public Task<ChineseIdCardBackOcrResult> RecognizeIdCardBack(Stream idCardBackImageData)
    {
        return Task.FromResult(new ChineseIdCardBackOcrResult
        {
            Issuer = "涿州市公安局",
            IssueDate = new DateTime(2013, 5, 5),
            ExpiresDate = null
        });
    }

    /// <summary>
    /// RecognizeIDCardFront
    /// </summary>
    /// <param name="idCardFrontImageData"></param>
    /// <returns></returns>
    public Task<ChineseIdCardFrontOcrResult> RecognizeIdCardFront(Stream idCardFrontImageData)
    {
        return Task.FromResult(new ChineseIdCardFrontOcrResult
        {
            DateOfBirth = new DateTime(161, 7, 16),
            Address = "河北省涿州市大树楼桑村",
            IdCardNumber = "130681016107160036",
            Name = "刘备",
            Nationality = "汉",
            SexString = "男"
        });
    }
}