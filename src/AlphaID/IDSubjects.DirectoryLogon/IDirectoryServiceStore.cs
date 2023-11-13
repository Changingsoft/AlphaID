namespace IDSubjects.DirectoryLogon;

/// <summary>
/// 
/// </summary>
public interface IDirectoryServiceStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<DirectoryService> Services { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    Task CreateAsync(DirectoryService service);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    Task UpdateAsync(DirectoryService service);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    Task DeleteAsync(DirectoryService service);

    /// <summary>
    /// Find by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DirectoryService?> FindByIdAsync(int id);
}
