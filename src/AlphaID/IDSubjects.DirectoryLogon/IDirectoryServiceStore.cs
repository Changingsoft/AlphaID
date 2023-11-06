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
    /// <param name="serivce"></param>
    /// <returns></returns>
    Task CreateAsync(DirectoryService serivce);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serivce"></param>
    /// <returns></returns>
    Task UpdateAsync(DirectoryService serivce);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serivce"></param>
    /// <returns></returns>
    Task DeleteAsync(DirectoryService serivce);

    /// <summary>
    /// Find by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DirectoryService?> FindByIdAsync(int id);
}
