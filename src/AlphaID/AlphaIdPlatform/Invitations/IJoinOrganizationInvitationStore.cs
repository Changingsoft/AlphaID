using AlphaIdPlatform.Subjects;
using IdSubjects;

namespace AlphaIdPlatform.Invitations;

/// <summary>
/// </summary>
public interface IJoinOrganizationInvitationStore
{
    /// <summary>
    /// </summary>
    IQueryable<JoinOrganizationInvitation> Invitations { get; }

    /// <summary>
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> CreateAsync(JoinOrganizationInvitation invitation);

    /// <summary>
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> UpdateAsync(JoinOrganizationInvitation invitation);

    /// <summary>
    /// </summary>
    /// <param name="invitation"></param>
    /// <returns></returns>
    Task<OrganizationOperationResult> DeleteAsync(JoinOrganizationInvitation invitation);
}