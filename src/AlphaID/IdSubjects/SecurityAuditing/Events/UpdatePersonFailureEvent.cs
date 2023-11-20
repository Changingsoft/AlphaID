namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// 更新自然人信息失败事件。
/// </summary>
internal class UpdatePersonFailureEvent : AuditLogEvent
{
    /// <summary>
    /// 
    /// </summary>
    public UpdatePersonFailureEvent() 
        : base(AuditLogEventCategories.AccountManagement,
               EventIds.UpdatePersonFailure,
               AuditLogEventTypes.Failure)
    {
    }
}
