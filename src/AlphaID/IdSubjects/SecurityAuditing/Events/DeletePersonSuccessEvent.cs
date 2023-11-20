namespace IdSubjects.SecurityAuditing.Events;
internal class DeletePersonSuccessEvent : AuditLogEvent
{
    public DeletePersonSuccessEvent()
        : base(AuditLogEventCategories.AccountManagement,
               EventIds.DeletePersonSuccess,
               AuditLogEventTypes.Success)
    {
    }
}