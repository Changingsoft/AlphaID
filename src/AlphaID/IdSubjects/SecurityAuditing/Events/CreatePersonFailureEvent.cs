namespace IdSubjects.SecurityAuditing.Events;
internal class CreatePersonFailureEvent() : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.CreatePersonFailure, AuditLogEventTypes.Failure);