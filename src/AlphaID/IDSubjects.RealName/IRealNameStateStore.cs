namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
public interface IRealNameStateStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    RealNameState? FindById(string id);
}