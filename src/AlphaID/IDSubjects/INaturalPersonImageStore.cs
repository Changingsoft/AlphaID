namespace IDSubjects;

/// <summary>
/// 提供访问用户图像的能力
/// </summary>
public interface INaturalPersonImageStore
{
    /// <summary>
    /// GetPhoto.
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task<(string MimeType, byte[] ImageContent)?> GetPhotoAsync(NaturalPerson person);

    /// <summary>
    /// Set Photo.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="mimeType"></param>
    /// <param name="imageContent"></param>
    /// <returns></returns>
    Task SetPhotoAsync(NaturalPerson person, string mimeType, byte[] imageContent);

    /// <summary>
    /// Clear Photo.
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task ClearPhotoAsync(NaturalPerson person);

}
