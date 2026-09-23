namespace AlphaId.InitData;

public static partial class InitData
{
    /// <summary>可访问受保护的公共 API。</summary>
    public static InitApiScope PublicApi { get; } = new(
        Name: "public",
        DisplayName: "公共API",
        Description: "可访问受保护的公共API",
        Created: Time("2023-02-10T13:22:00.0000000"));

    /// <summary>获取自然人的实名制信息，如身份证号码。</summary>
    public static InitApiScope RealName { get; } = new(
        Name: "realname",
        DisplayName: "实名信息",
        Description: "获取自然人的实名制信息，如身份证号码",
        Created: Time("2023-02-08T14:32:00.0000000"),
        Emphasize: true);

    /// <summary>获取用户的组织成员身份。</summary>
    public static InitApiScope Membership { get; } = new(
        Name: "membership",
        DisplayName: "组织成员",
        Description: "获取用户的组织成员身份",
        Created: Time("2023-02-08T14:32:00.0000000"),
        Emphasize: true);

    /// <summary>全部内置 API 范围。</summary>
    public static IReadOnlyList<InitApiScope> ApiScopes { get; } =
    [
        PublicApi,
        RealName,
        Membership,
    ];
}
