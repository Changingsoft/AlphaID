namespace IdSubjects.SecurityAuditing.Events;

internal class ChangePasswordFailureEvent(string userName, string message) : AuditLogEvent(
    AuditLogEventCategories.AccountManagement,
    EventIds.ChangePasswordFailure, AuditLogEventTypes.Failure, message)
{
    public string UserName { get; set; } = userName;
}