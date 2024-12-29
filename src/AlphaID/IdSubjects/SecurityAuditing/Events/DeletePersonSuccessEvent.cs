namespace IdSubjects.SecurityAuditing.Events;

internal class DeletePersonSuccessEvent(string? userName) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.DeletePersonSuccess,
    AuditLogEventTypes.Success)
{
    public string? UserName { get; set; } = userName;
}