namespace IDSubjects;

/// <summary>
/// 用于指示某物被视为个人数据。
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class PersonalDataAttribute : Attribute
{
}
