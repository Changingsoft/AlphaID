namespace IdSubjects.SecurityAuditing.Events;

internal class DeletePersonFailureEvent() : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.DeletePersonFailure,
    AuditLogEventTypes.Failure);