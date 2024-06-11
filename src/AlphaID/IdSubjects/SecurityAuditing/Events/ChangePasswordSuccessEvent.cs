namespace IdSubjects.SecurityAuditing.Events;

internal class ChangePasswordSuccessEvent(string userName, string message) : AuditLogEvent(
    AuditLogEventCategories.AccountManagement,
    EventIds.ChangePasswordSuccess, AuditLogEventTypes.Success, message)
{
    public string UserName { get; set; } = userName;
}