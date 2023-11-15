namespace IdSubjects.Diagnostics;

/// <summary>
/// 
/// </summary>
public abstract class NaturalPersonManagerInterceptor : INaturalPersonManagerInterceptor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PreCreateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Task PostCreateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PreUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PostUpdateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PreDeleteAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PostDeleteAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
    }
}