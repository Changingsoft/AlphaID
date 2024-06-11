namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
///     更新自然人信息失败事件。
/// </summary>
internal class UpdatePersonFailureEvent : AuditLogEvent
{
    /// <summary>
    /// </summary>
    public UpdatePersonFailureEvent(string userName)
        : base(AuditLogEventCategories.AccountManagement,
            EventIds.UpdatePersonFailure,
            AuditLogEventTypes.Failure,
            "更新自然人信息失败。")
    {
        UserName = userName;
    }

    public string UserName { get; set; }
}