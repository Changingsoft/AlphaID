namespace IDSubjects.RealName;

/// <summary>
/// 
/// </summary>
public interface IRealNameStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    RealNameInfo? FindByPersonId(string id);
}