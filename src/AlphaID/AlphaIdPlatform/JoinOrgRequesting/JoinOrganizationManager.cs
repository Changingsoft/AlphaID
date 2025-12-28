using Organizational;

namespace AlphaIdPlatform.JoinOrgRequesting;

/// <summary>
/// Manages operations related to joining an organization, including creating, auditing, and canceling join requests.
/// </summary>
/// <remarks>This class provides methods to handle join organization requests, including creating new requests,
/// auditing them  (to approve or reject), and canceling existing requests. It interacts with the underlying request
/// store and  organization management system to perform these operations.</remarks>
/// <param name="store"></param>
/// <param name="logger"></param>
/// <param name="organizationStore"></param>
public class JoinOrganizationManager(
    IJoinOrganizationRequestStore store,
    ILogger<JoinOrganizationManager>? logger,
    IOrganizationStore organizationStore)
{
    /// <summary>
    /// Creates a new organization join request asynchronously.
    /// </summary>
    /// <param name="request">The request containing the details of the organization to join. Cannot be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Create(JoinOrganizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        await store.CreateAsync(request);
    }

    /// <summary>
    /// Audits a user's request to join an organization, updating the request status and organization membership as
    /// necessary.
    /// </summary>
    /// <remarks>If the request is accepted, the user is added to the organization with the specified
    /// membership details. If the request is rejected, no changes are made to the organization, and the rejection is
    /// logged.</remarks>
    /// <param name="request">The request to join the organization. Cannot be <see langword="null"/>.</param>
    /// <param name="auditor">The identifier of the auditor performing the review. Cannot be <see langword="null"/> or whitespace.</param>
    /// <param name="accepted">A value indicating whether the request is accepted. If <see langword="true"/>, the user will be added to the
    /// organization; otherwise, the request will be marked as rejected.</param>
    /// <param name="visibility">The visibility level of the user's membership within the organization. Defaults to <see
    /// cref="MembershipVisibility.Public"/>.</param>
    /// <param name="title">An optional title for the user within the organization. Can be <see langword="null"/>.</param>
    /// <param name="department">An optional department for the user within the organization. Can be <see langword="null"/>.</param>
    /// <param name="remark">An optional remark associated with the user's membership. Can be <see langword="null"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is <see langword="null"/> or if <paramref name="auditor"/> is <see
    /// langword="null"/> or whitespace.</exception>
    public async Task Audit(JoinOrganizationRequest request,
        string auditor,
        bool accepted,
        MembershipVisibility visibility = MembershipVisibility.Public,
        string? title = null,
        string? department = null,
        string? remark = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(auditor))
            throw new ArgumentNullException(nameof(auditor));
        request.AuditAt = DateTimeOffset.UtcNow;
        request.AuditBy = auditor;
        request.IsAccepted = accepted;
        await store.UpdateAsync(request);

        if (!accepted)
        {
            logger?.LogDebug("{user}加入组织{organization}的请求审核未通过。审核人是{auditor}。", request.UserId, request.OrganizationName, auditor);
            return;
        }

        var org = await organizationStore.FindByIdAsync(request.OrganizationId);
        if (org == null)
        {
            logger?.LogWarning("{organization} not found in join request. May be it would been deleted before audit.", request.OrganizationName);
            return;
        }

        if (org.Members.Any(m => m.PersonId == request.UserId))
        {
            logger?.LogDebug("{user} has already in {organization}.", request.UserId, request.OrganizationName);
            return;
        }

        org.Members.Add(new OrganizationMember(request.UserId, visibility)
        {
            Title = title,
            Department = department,
            Remark = remark,
        });
        await organizationStore.UpdateAsync(org);
    }

    /// <summary>
    /// Cancels the specified join organization request.
    /// </summary>
    /// <param name="request">The request to be canceled. Cannot be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous operation of canceling the request.</returns>
    public async Task Cancel(JoinOrganizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        await store.DeleteAsync(request);
    }
}
