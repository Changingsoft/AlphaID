namespace IdSubjects.SecurityAuditing.Events;

internal class ChangePasswordFailureEvent : AuditLogEvent
{
    public ChangePasswordFailureEvent(string message)
        : base(AuditLogEventCategories.AccountManagement, EventIds.ChangePasswordFailure, AuditLogEventTypes.Failure, message)
    {
        throw new NotImplementedException();
    }
}