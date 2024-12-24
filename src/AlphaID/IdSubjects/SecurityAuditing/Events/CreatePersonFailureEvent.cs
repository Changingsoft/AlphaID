using Microsoft.AspNetCore.Identity;

namespace IdSubjects.SecurityAuditing.Events;

internal class CreatePersonFailureEvent(string userName, IEnumerable<IdentityError> errors) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.CreatePersonFailure, AuditLogEventTypes.Failure, "创建自然人失败。")
{
    public string UserName { get; set; } = userName;

    public IEnumerable<IdentityError> Errors { get; set; } = errors;
}