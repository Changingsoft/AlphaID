namespace IdSubjects.SecurityAuditing.Events;

internal class ChangePasswordSuccessEvent(string message) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
    EventIds.ChangePasswordSuccess, AuditLogEventTypes.Success, message);