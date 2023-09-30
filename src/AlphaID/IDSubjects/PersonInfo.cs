namespace IDSubjects;

/// <summary>
/// 表示一个自然人信息。
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="PhoneticSearchHint"></param>
public record PersonInfo(string Id, string Name, string? PhoneticSearchHint);
