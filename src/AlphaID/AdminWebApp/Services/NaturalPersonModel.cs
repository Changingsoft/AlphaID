namespace AdminWebApp.Services;

public record NaturalPersonModel(string SubjectId,
                          string Name,
                          string? Sex,
                          string? MobilePhoneSuffix,
                          bool RealNameValid,
                          string? PhoneticSearchHint,
                          IEnumerable<string> MembersOfHint);