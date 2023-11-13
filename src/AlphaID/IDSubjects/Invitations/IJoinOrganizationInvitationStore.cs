namespace IDSubjects.Invitations;

/// <summary>
/// 
/// </summary>
public interface IJoinOrganizationInvitationStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<JoinOrganizationInvitation> Invitations { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(JoinOrganizationInvitation invitation);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(JoinOrganizationInvitation invitation);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(JoinOrganizationInvitation invitation);
}