namespace IdSubjects.ChineseName;

/// <summary>
///     针对<see cref="ApplicationUserBuilder{T}" />的扩展。
/// </summary>
public static class PersonBuilderExtensions
{
    /// <summary>
    ///     使用中国人名称。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="chinesePersonName">中国人名称。</param>
    /// <returns></returns>
    public static ApplicationUserBuilder<T> UseChinesePersonName<T>(this ApplicationUserBuilder<T> builder, ChinesePersonName chinesePersonName)
    where T : ApplicationUser, new()
    {
        var personName = new HumanNameInfo(chinesePersonName.FullName, chinesePersonName.Surname,
            chinesePersonName.GivenName);
        builder.SetPersonName(personName);
        return builder;
    }
}