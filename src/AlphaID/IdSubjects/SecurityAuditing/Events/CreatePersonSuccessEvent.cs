namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// 表示一个登录成功事件。
/// </summary>
/// <remarks>
/// </remarks>
internal class CreatePersonSuccessEvent(string? userName) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
        EventIds.CreatePersonSuccess,
        AuditLogEventTypes.Success,
        "成功创建了自然人。")
{
    public string? UserName { get; set; } = userName;
}