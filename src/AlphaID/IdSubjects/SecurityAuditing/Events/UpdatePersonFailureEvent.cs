namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// 更新自然人信息失败事件。
/// </summary>
/// <remarks>
/// </remarks>
internal class UpdatePersonFailureEvent(string? userName) : AuditLogEvent(AuditLogEventCategories.AccountManagement,
        EventIds.UpdatePersonFailure,
        AuditLogEventTypes.Failure,
        "更新自然人信息失败。")
{
    public string? UserName { get; set; } = userName;
}