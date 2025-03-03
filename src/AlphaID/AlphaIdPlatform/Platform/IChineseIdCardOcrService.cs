namespace AlphaIdPlatform.Platform;

/// <summary>
/// </summary>
public interface IChineseIdCardOcrService
{
    /// <summary>
    /// 根据指定图像，识别身份证正面信息。
    /// </summary>
    /// <param name="idCardFrontImageData"></param>
    /// <returns></returns>
    Task<ChineseIdCardFrontOcrResult> RecognizeIdCardFront(Stream idCardFrontImageData);

    /// <summary>
    /// 根据指定图像，识别身份证背面信息。
    /// </summary>
    /// <param name="idCardBackImageData"></param>
    /// <returns></returns>
    Task<ChineseIdCardBackOcrResult> RecognizeIdCardBack(Stream idCardBackImageData);
}