namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
///     更新用户信息成功事件。
/// </summary>
internal class UpdatePersonSuccessEvent : AuditLogEvent
{
    /// <summary>
    /// </summary>
    public UpdatePersonSuccessEvent()
        : base(AuditLogEventCategories.AccountManagement,
            EventIds.UpdatePersonSuccess,
            AuditLogEventTypes.Success)
    {
    }
}