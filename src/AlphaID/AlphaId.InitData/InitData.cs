using System.Globalization;

namespace AlphaId.InitData;

/// <summary>
/// 描述一条内置 API 范围（<c>ApiScopes</c>）。
/// </summary>
/// <param name="Name">范围名称，同时是内置数据的自然键。</param>
/// <param name="DisplayName">显示名称。</param>
/// <param name="Description">说明。</param>
/// <param name="Created">创建时间。</param>
/// <param name="Required">是否必须授予。</param>
/// <param name="Emphasize">是否在同意页面上强调。</param>
/// <remarks>
/// 未在此列出的字段一律取 IdentityServer 实体自身的默认值（例如 <c>Enabled</c>、
/// <c>ShowInDiscoveryDocument</c> 为 true，<c>LastAccessed</c> 为 NULL）。这些默认值与
/// 历史 T-SQL 脚本写入的值一致，已由归一化行比对验证；改动前请先确认实体的默认值没有变化。
/// </remarks>
public sealed record InitApiScope(
    string Name,
    string DisplayName,
    string Description,
    DateTime Created,
    bool Required = false,
    bool Emphasize = false);

/// <summary>
/// 描述一条内置标识资源（<c>IdentityResources</c>）。
/// </summary>
/// <param name="Name">资源名称，同时是内置数据的自然键。</param>
/// <param name="DisplayName">显示名称。</param>
/// <param name="Description">说明。</param>
/// <param name="Created">创建时间。</param>
/// <param name="UserClaims">该资源签发的声明类型。</param>
/// <param name="Required">是否必须授予。</param>
/// <param name="Emphasize">是否在同意页面上强调。</param>
public sealed record InitIdentityResource(
    string Name,
    string DisplayName,
    string Description,
    DateTime Created,
    IReadOnlyList<string>? UserClaims = null,
    bool Required = false,
    bool Emphasize = false);

/// <summary>
/// 描述一个内置客户端的密钥。
/// </summary>
/// <param name="PlainText">
/// 密钥明文。必须与引用该客户端的应用配置中的 <c>ClientSecret</c> 完全一致，
/// 入库时由 <see cref="InitDataSeeder.HashSecret"/> 按 IdentityServer 对
/// <c>SharedSecret</c> 的默认约定派生为哈希。
/// </param>
/// <param name="Created">创建时间。</param>
public sealed record InitClientSecret(string PlainText, DateTime Created);

/// <summary>
/// 描述一个内置客户端（<c>Clients</c>）。
/// </summary>
/// <param name="ClientId">客户端标识（GUID 字符串），同时是内置数据的自然键。</param>
/// <param name="ClientName">客户端名称。</param>
/// <param name="Created">创建时间。</param>
/// <param name="Secret">客户端密钥。</param>
/// <param name="Description">说明。</param>
/// <param name="GrantTypes">允许的授权类型。</param>
/// <param name="RedirectUris">允许的重定向地址。</param>
/// <param name="PostLogoutRedirectUris">注销后允许的重定向地址。</param>
/// <param name="Scopes">允许请求的范围。</param>
/// <param name="CorsOrigins">允许的跨域来源。</param>
/// <param name="AllowOfflineAccess">是否允许离线访问（refresh token）。</param>
/// <remarks>
/// 未在此列出的字段一律取 IdentityServer 实体自身的默认值。其中值得留意的是
/// <c>DPoPValidationMode</c>：实体默认值为 <c>Custom</c>，而历史 T-SQL 脚本为内置客户端
/// 写入了 <c>Iat</c>，因此该字段由 <see cref="InitDataSeeder"/> 统一显式设置，而不在数据中表达。
/// </remarks>
public sealed record InitClient(
    string ClientId,
    string ClientName,
    DateTime Created,
    InitClientSecret? Secret = null,
    string? Description = null,
    IReadOnlyList<string>? GrantTypes = null,
    IReadOnlyList<string>? RedirectUris = null,
    IReadOnlyList<string>? PostLogoutRedirectUris = null,
    IReadOnlyList<string>? Scopes = null,
    IReadOnlyList<string>? CorsOrigins = null,
    bool AllowOfflineAccess = false);

/// <summary>
/// Alpha ID 的内置数据（初始化数据）。系统未安装这些数据时无法正常工作，
/// 没有界面可以编辑或删除它们。
/// </summary>
/// <remarks>
/// <para>
/// 内置数据的定义源从 <c>DatabaseTool/InitData/**/*.sql</c> 迁移到本类型。原先的 T-SQL 脚本
/// 通过 <c>ExecuteSqlRawAsync</c> 原样执行，不具备幂等性：对已有数据库再次运行数据库工具时
/// 会因主键冲突而崩溃。现在的定义是 C# 常量，由 <see cref="InitDataSeeder"/> 以 Ensure 语义落地。
/// </para>
/// <para>
/// <b>这些数值与历史脚本写入的数据库是等价的</b>：连 IdentityServer 客户端的密钥哈希、
/// 各行的创建时间都被刻意保留。修改前请先阅读 <c>docs/InitData.md</c>。
/// </para>
/// <para>
/// 注意：本类型的简单名 <c>InitData</c> 与直属命名空间 <c>AlphaId.InitData</c> 同名。
/// 在 <c>AlphaId.*</c> 命名空间内**直接**书写 <c>InitData.ApiScopes</c> 时，
/// <c>InitData</c> 会被解析为命名空间而非本类型；请改用 <c>global::AlphaId.InitData.InitData</c>
/// 或把调用点放在其他命名空间（例如 <c>DatabaseTool.Migrators</c>）。
/// </para>
/// </remarks>
public static partial class InitData
{
    /// <summary>Admin Center 客户端的密钥明文。</summary>
    public const string ManagementCenterClientSecret = "i7zcwJu)5pgIA()huJWRoT@oCLHpwfe^";

    /// <summary>AuthCenter Swagger UI 客户端的密钥明文。</summary>
    public const string SwaggerUiClientSecret = "FwXOsol6K0YYF5kzC3RXD9ynlIc+nI/o";

    /// <summary>把历史脚本中的时间字面量解析为 <see cref="DateTime"/>。</summary>
    /// <param name="value">形如 <c>2023-02-10T13:22:00.0000000</c> 的时间字符串。</param>
    /// <remarks>使用 <see cref="DateTimeStyles.RoundtripKind"/> 保留 <c>o</c> 格式的精度。</remarks>
    internal static DateTime Time(string value) =>
        DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
}
