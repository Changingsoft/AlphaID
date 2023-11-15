namespace IdSubjects.Diagnostics;

/// <summary>
/// 
/// </summary>
public interface INaturalPersonManagerInterceptor : IInterceptor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PreCreateAsync(NaturalPersonManager personManager, NaturalPerson person);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PostCreateAsync(NaturalPersonManager personManager, NaturalPerson person);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PreUpdateAsync(NaturalPersonManager personManager, NaturalPerson person);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PostUpdateAsync(NaturalPersonManager personManager, NaturalPerson person);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PreDeleteAsync(NaturalPersonManager personManager, NaturalPerson person);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PostDeleteAsync(NaturalPersonManager personManager, NaturalPerson person);
}