namespace IdSubjects.SecurityAuditing.Events;

internal class DeletePersonSuccessEvent() : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.DeletePersonSuccess,
    AuditLogEventTypes.Success);