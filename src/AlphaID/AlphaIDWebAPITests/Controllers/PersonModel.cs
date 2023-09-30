namespace AlphaIDWebAPITests.Controllers;
internal class PersonModel
{
    public string SubjectId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Sex { get; set; }
    public string? MobilePhoneSuffix { get; set; }
    public bool RealNameValid { get; set; }
    public string? PhoneticSearchHint { get; set; }
}
