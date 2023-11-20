namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// 表示一个登录成功事件。
/// </summary>
internal class CreatePersonSuccessEvent : AuditLogEvent
{
    /// <summary>
    /// 
    /// </summary>
    public CreatePersonSuccessEvent()
        : base(AuditLogEventCategories.AccountManagement,
               EventIds.CreatePersonSuccess,
               AuditLogEventTypes.Success)
    {
    }
}
