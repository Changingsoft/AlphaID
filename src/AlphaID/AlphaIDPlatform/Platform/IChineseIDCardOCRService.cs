namespace AlphaIDPlatform.Platform;

/// <summary>
/// 
/// </summary>
public interface IChineseIDCardOCRService
{
    /// <summary>
    /// 根据指定图像，识别身份证正面信息。
    /// </summary>
    /// <param name="idCardFrontImageData"></param>
    /// <returns></returns>
    Task<ChineseIDCardFrontOCRResult> RecognizeIDCardFront(Stream idCardFrontImageData);

    /// <summary>
    /// 根据指定图像，识别身份证背面信息。
    /// </summary>
    /// <param name="idCardBackImageData"></param>
    /// <returns></returns>
    Task<ChineseIDCardBackOCRResult> RecognizeIDCardBack(Stream idCardBackImageData);
}
