namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
///     更新用户信息成功事件。
/// </summary>
/// <remarks>
/// </remarks>
internal class UpdatePersonSuccessEvent(string? userName) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
        EventIds.UpdatePersonSuccess,
        AuditLogEventTypes.Success,
        "成功更新了自然人信息。")
{
    public string? UserName { get; set; } = userName;
}