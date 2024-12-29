namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
///     更新用户信息成功事件。
/// </summary>
internal class UpdatePersonSuccessEvent : AuditLogEvent
{
    /// <summary>
    /// </summary>
    public UpdatePersonSuccessEvent(string? userName)
        : base(AuditLogEventCategories.AccountManagement,
            EventIds.UpdatePersonSuccess,
            AuditLogEventTypes.Success,
            "成功更新了自然人信息。")
    {
        UserName = userName;
    }

    public string? UserName { get; set; }
}