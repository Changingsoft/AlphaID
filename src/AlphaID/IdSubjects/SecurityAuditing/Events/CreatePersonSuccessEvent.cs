namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
///     表示一个登录成功事件。
/// </summary>
internal class CreatePersonSuccessEvent : AuditLogEvent
{
    /// <summary>
    /// </summary>
    public CreatePersonSuccessEvent(string? userName)
        : base(AuditLogEventCategories.AccountManagement,
            EventIds.CreatePersonSuccess,
            AuditLogEventTypes.Success,
            "成功创建了自然人。")
    {
        UserName = userName;
    }

    public string? UserName { get; set; }
}