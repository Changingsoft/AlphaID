namespace AlphaId.TestingData;

/// <content>
/// <see cref="SampleData"/> 的样例角色分配。
/// </content>
public static partial class SampleData
{
    /// <summary>把刘备设为管理后台的 Administrators 角色成员。</summary>
    public static SampleUserInRole LiuBeiIsAdministrator { get; } = new()
    {
        UserId = "d2480421-8a15-4292-8e8f-06985a1f645b",
        RoleName = "Administrators",
    };

    /// <summary>全部样例角色分配。</summary>
    public static IReadOnlyList<SampleUserInRole> UserInRoles =>
    [
        LiuBeiIsAdministrator,
    ];
}
