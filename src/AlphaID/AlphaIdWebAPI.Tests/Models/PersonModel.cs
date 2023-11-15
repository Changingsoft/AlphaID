namespace AlphaIdWebAPI.Tests.Models;
internal record PersonModel(string? Sex, string? MobilePhoneSuffix, bool RealNameValid, string? PhoneticSearchHint)
{
    public string SubjectId { get; set; } = default!;
    public string Name { get; set; } = default!;
}
