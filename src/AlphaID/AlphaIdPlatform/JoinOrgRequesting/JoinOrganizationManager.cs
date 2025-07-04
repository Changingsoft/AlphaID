using AlphaIdPlatform.Subjects;

namespace AlphaIdPlatform.JoinOrgRequesting;


public class JoinOrganizationManager(
    IJoinOrganizationRequestStore store,
    ILogger<JoinOrganizationManager>? logger,
    OrganizationManager organizationManager)
{
    public Task Create(JoinOrganizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return store.CreateAsync(request);
    }

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

        var org = await organizationManager.FindByIdAsync(request.OrganizationId);
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
        await organizationManager.UpdateAsync(org);
    }

    public Task Cancel(JoinOrganizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return store.DeleteAsync(request);
    }
}
