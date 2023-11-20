namespace IdSubjects.SecurityAuditing.Events;

internal class ChangePasswordSuccessEvent : AuditLogEvent
{
    public ChangePasswordSuccessEvent(string message)
        : base(AuditLogEventCategories.AccountManagement, EventIds.ChangePasswordSuccess, AuditLogEventTypes.Success, message)
    {
    }
}