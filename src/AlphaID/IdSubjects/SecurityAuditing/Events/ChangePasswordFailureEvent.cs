namespace IdSubjects.SecurityAuditing.Events;

internal class ChangePasswordFailureEvent(string message) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.ChangePasswordFailure, AuditLogEventTypes.Failure, message);