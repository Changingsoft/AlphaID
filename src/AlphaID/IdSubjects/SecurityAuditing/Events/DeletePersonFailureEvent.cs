namespace IdSubjects.SecurityAuditing.Events;

internal class DeletePersonFailureEvent(string userName) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.DeletePersonFailure,
    AuditLogEventTypes.Failure)
{
    public string UserName { get; set; } = userName;
}