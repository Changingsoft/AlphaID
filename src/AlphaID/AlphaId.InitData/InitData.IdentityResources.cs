namespace AlphaId.InitData;

public static partial class InitData
{
    /// <summary>您的用户标识符。</summary>
    public static InitIdentityResource OpenId { get; } = new(
        Name: "openid",
        DisplayName: "您的用户标识符",
        Description: "您的Id",
        Created: Time("2022-12-15T20:27:25.2378862"),
        UserClaims: ["sub"],
        Required: true);

    /// <summary>您的基本信息，如姓名等。</summary>
    public static InitIdentityResource Profile { get; } = new(
        Name: "profile",
        DisplayName: "用户配置文件",
        Description: "您的基本信息，如姓名等",
        Created: Time("2022-12-15T20:27:25.2681944"),
        UserClaims:
        [
            "name",
            "family_name",
            "given_name",
            "middle_name",
            "nickname",
            "preferred_username",
            "profile",
            "picture",
            "website",
            "gender",
            "birthdate",
            "zoneinfo",
            "locale",
            "updated_at",
            "search_hint",
        ],
        Emphasize: true);

    /// <summary>您的电子邮件地址。</summary>
    public static InitIdentityResource Email { get; } = new(
        Name: "email",
        DisplayName: "您的电子邮件地址",
        Description: "您的电子邮件地址",
        Created: Time("2022-12-15T20:27:25.2681944"),
        UserClaims: ["email", "email_verified"],
        Emphasize: true);

    /// <summary>您的邮政地址。</summary>
    public static InitIdentityResource Address { get; } = new(
        Name: "address",
        DisplayName: "您的邮政地址",
        Description: "您的邮政地址",
        Created: Time("2022-12-15T20:27:25.2681944"),
        UserClaims: ["address"],
        Emphasize: true);

    /// <summary>您的手机号。</summary>
    public static InitIdentityResource Phone { get; } = new(
        Name: "phone",
        DisplayName: "您的手机号",
        Description: "您的手机号",
        Created: Time("2022-12-15T20:27:25.2681944"),
        UserClaims: ["phone_number", "phone_number_verified"],
        Emphasize: true);

    /// <summary>全部内置标识资源。顺序即写入顺序，与历史脚本一致。</summary>
    public static IReadOnlyList<InitIdentityResource> IdentityResources { get; } =
    [
        OpenId,
        Profile,
        Email,
        Address,
        Phone,
    ];
}
