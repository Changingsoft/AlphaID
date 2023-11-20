namespace IdSubjects.SecurityAuditing.Events;
internal class DeletePersonFailureEvent : AuditLogEvent
{
    public DeletePersonFailureEvent()
        : base(AuditLogEventCategories.AccountManagement,
               EventIds.DeletePersonFailure,
               AuditLogEventTypes.Failure)
    {
    }
}