namespace IdSubjects.DirectoryLogon;

/// <summary>
/// 
/// </summary>
public interface IDirectoryServiceDescriptorStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<DirectoryServiceDescriptor> Services { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceDescriptor"></param>
    /// <returns></returns>
    Task CreateAsync(DirectoryServiceDescriptor serviceDescriptor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceDescriptor"></param>
    /// <returns></returns>
    Task UpdateAsync(DirectoryServiceDescriptor serviceDescriptor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceDescriptor"></param>
    /// <returns></returns>
    Task DeleteAsync(DirectoryServiceDescriptor serviceDescriptor);

    /// <summary>
    /// Find by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DirectoryServiceDescriptor?> FindByIdAsync(int id);
}
