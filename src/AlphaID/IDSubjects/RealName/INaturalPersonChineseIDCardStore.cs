namespace IDSubjects.RealName;

/// <summary>
/// 提供对身份证的管理能力
/// </summary>
public interface INaturalPersonChineseIDCardStore : INaturalPersonStore
{
    /// <summary>
    /// 根据身份证号码查找指定的自然人。
    /// </summary>
    /// <param name="chineseIDCardNumber"></param>
    /// <returns></returns>
    Task<NaturalPerson?> FindByChineseIDCardNumberAsync(string chineseIDCardNumber);

    /// <summary>
    /// 获取自然人的身份证信息。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task<PersonChineseIDCard?> GetChineseIDCardInfoAsync(NaturalPerson person);

    /// <summary>
    /// 设置指定自然人的身份证信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="chineseIDCard"></param>
    /// <returns></returns>
    Task SetChineseIDCardInfoAsync(NaturalPerson person, PersonChineseIDCard chineseIDCard);

    /// <summary>
    /// 清除指定自然人的身份证信息。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task ClearChineseIDCardInfoAsync(NaturalPerson person);
}
