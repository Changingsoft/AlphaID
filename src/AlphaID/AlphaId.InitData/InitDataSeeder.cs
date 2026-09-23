using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using DPoPValidationMode = Duende.IdentityServer.Models.DPoPTokenExpirationValidationMode;

namespace AlphaId.InitData;

/// <summary>
/// 控制 <see cref="InitDataSeeder"/> 的写入行为。
/// </summary>
public sealed record InitDataEnsureOptions
{
    /// <summary>默认行为：只补缺失的内置数据，已存在的保持原样。</summary>
    public static InitDataEnsureOptions Default { get; } = new();

    /// <summary>
    /// 为 <see langword="true"/> 时，把已存在且与仓库定义不一致的内置行覆盖回定义值
    /// （含子行：授权类型、重定向地址、范围、密钥等会被整体替换为定义中的集合）。
    /// </summary>
    /// <remarks>
    /// 这会冲掉运维在数据库中对手工调整。只在内置数据本身需要升级时使用，
    /// 例如 IdentityServer 升级后需要补齐新版本才认识的字段。
    /// </remarks>
    public bool OverwriteExisting { get; init; }
}

/// <summary>
/// <see cref="InitDataSeeder"/> 的执行结果。
/// </summary>
public sealed class InitDataResult
{
    private readonly List<string> _warnings = [];

    /// <summary>新插入的内置项数量（不含子行）。</summary>
    public int Inserted { get; internal set; }

    /// <summary>因已存在而跳过的内置项数量（不含子行）。</summary>
    public int Skipped { get; internal set; }

    /// <summary>已存在的内置项与仓库定义之间的差异告警。</summary>
    public IReadOnlyList<string> Warnings => _warnings;

    /// <summary>本次执行是否写入了数据。</summary>
    public bool HasChanges => Inserted > 0;

    internal void Warn(string message) => _warnings.Add(message);

    internal void Absorb(InitDataResult other)
    {
        Inserted += other.Inserted;
        Skipped += other.Skipped;
        _warnings.AddRange(other._warnings);
    }
}

/// <summary>
/// 把 <see cref="InitData"/> 中声明的内置数据以「确保存在」的方式写入 IdentityServer 配置库。
/// </summary>
/// <remarks>
/// <para>
/// 与样例数据（<c>AlphaId.TestingData</c> + <c>SampleDataSeeder</c>）不同，内置数据必须能反复写入：
/// 数据库工具经常在已有数据库上再次运行，而 <c>docs/InitData.md</c> 明确说明运维可以通过数据库
/// 直接编辑这些数据。因此这里采用 <b>Ensure</b> 语义：
/// </para>
/// <list type="number">
/// <item><description>按自然键判断内置项是否存在；不存在则连同子行一并插入。</description></item>
/// <item><description>已存在则保持原样（尊重运维的手工调整），只在字段与仓库定义不一致时给出告警。</description></item>
/// <item><description>需要强制对齐时显式传入 <see cref="InitDataEnsureOptions.OverwriteExisting"/>。</description></item>
/// </list>
/// <para>
/// 自然键为 <c>Client.ClientId</c>（GUID 字符串）、<c>ApiScope.Name</c> 与 <c>IdentityResource.Name</c>。
/// 数值主键 <c>Id</c> 由数据库自增，不属于内置数据的语义，也不需要与历史库一致。
/// </para>
/// </remarks>
public static class InitDataSeeder
{
    /// <summary>确保全部内置数据存在。</summary>
    /// <param name="db">IdentityServer 的配置库上下文。</param>
    /// <param name="options">写入行为选项，为 <see langword="null"/> 时使用 <see cref="InitDataEnsureOptions.Default"/>。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>写入结果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="db"/> 为 <see langword="null"/>。</exception>
    public static async Task<InitDataResult> EnsureAsync(
        ConfigurationDbContext db,
        InitDataEnsureOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await EnsureCoreAsync(db, options, cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException)
        {
            // 并发的调用方（例如同时运行的数据库工具与测试进程）可能刚刚写入了同一批内置数据，
            // 此时唯一键冲突并不代表失败。丢弃本次的跟踪状态后重试一次，这些行会被当作「已存在」。
            db.ChangeTracker.Clear();
            return await EnsureCoreAsync(db, options, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>确保指定的内置 API 范围存在。</summary>
    /// <param name="db">IdentityServer 的配置库上下文。</param>
    /// <param name="apiScopes">要确保的范围，为 <see langword="null"/> 时使用 <see cref="InitData.ApiScopes"/>。</param>
    /// <param name="options">写入行为选项，为 <see langword="null"/> 时使用 <see cref="InitDataEnsureOptions.Default"/>。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>写入结果。</returns>
    public static async Task<InitDataResult> EnsureApiScopesAsync(
        ConfigurationDbContext db,
        IEnumerable<InitApiScope>? apiScopes = null,
        InitDataEnsureOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        options ??= InitDataEnsureOptions.Default;
        IReadOnlyList<InitApiScope> definitions = apiScopes as IReadOnlyList<InitApiScope> ?? apiScopes?.ToList() ?? InitData.ApiScopes;
        InitDataResult result = new();

        Dictionary<string, ApiScope> existing = await db.ApiScopes
            .ToDictionaryAsync(scope => scope.Name, StringComparer.Ordinal, cancellationToken)
            .ConfigureAwait(false);

        foreach (InitApiScope definition in definitions)
        {
            if (existing.TryGetValue(definition.Name, out ApiScope? scope))
            {
                result.Skipped++;
                if (options.OverwriteExisting)
                    Apply(scope, definition);
                else
                    CheckApiScope(scope, definition, result);
                continue;
            }

            db.ApiScopes.Add(Create(definition));
            result.Inserted++;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <summary>确保指定的内置标识资源存在。</summary>
    /// <param name="db">IdentityServer 的配置库上下文。</param>
    /// <param name="identityResources">要确保的标识资源，为 <see langword="null"/> 时使用 <see cref="InitData.IdentityResources"/>。</param>
    /// <param name="options">写入行为选项，为 <see langword="null"/> 时使用 <see cref="InitDataEnsureOptions.Default"/>。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>写入结果。</returns>
    public static async Task<InitDataResult> EnsureIdentityResourcesAsync(
        ConfigurationDbContext db,
        IEnumerable<InitIdentityResource>? identityResources = null,
        InitDataEnsureOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        options ??= InitDataEnsureOptions.Default;
        IReadOnlyList<InitIdentityResource> definitions =
            identityResources as IReadOnlyList<InitIdentityResource> ?? identityResources?.ToList() ?? InitData.IdentityResources;
        InitDataResult result = new();

        Dictionary<string, IdentityResource> existing = await db.IdentityResources
            .Include(resource => resource.UserClaims)
            .ToDictionaryAsync(resource => resource.Name, StringComparer.Ordinal, cancellationToken)
            .ConfigureAwait(false);

        foreach (InitIdentityResource definition in definitions)
        {
            if (existing.TryGetValue(definition.Name, out IdentityResource? resource))
            {
                result.Skipped++;
                if (options.OverwriteExisting)
                    Apply(resource, definition);
                else
                    CheckIdentityResource(resource, definition, result);
                continue;
            }

            db.IdentityResources.Add(Create(definition));
            result.Inserted++;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <summary>确保指定的内置客户端存在。</summary>
    /// <param name="db">IdentityServer 的配置库上下文。</param>
    /// <param name="clients">要确保的客户端，为 <see langword="null"/> 时使用 <see cref="InitData.Clients"/>。</param>
    /// <param name="options">写入行为选项，为 <see langword="null"/> 时使用 <see cref="InitDataEnsureOptions.Default"/>。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>写入结果。</returns>
    public static async Task<InitDataResult> EnsureClientsAsync(
        ConfigurationDbContext db,
        IEnumerable<InitClient>? clients = null,
        InitDataEnsureOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        options ??= InitDataEnsureOptions.Default;
        IReadOnlyList<InitClient> definitions = clients as IReadOnlyList<InitClient> ?? clients?.ToList() ?? InitData.Clients;
        InitDataResult result = new();

        Dictionary<string, Client> existing = await db.Clients
            .Include(client => client.AllowedGrantTypes)
            .Include(client => client.AllowedScopes)
            .Include(client => client.AllowedCorsOrigins)
            .Include(client => client.RedirectUris)
            .Include(client => client.PostLogoutRedirectUris)
            .Include(client => client.ClientSecrets)
            .ToDictionaryAsync(client => client.ClientId, StringComparer.Ordinal, cancellationToken)
            .ConfigureAwait(false);

        foreach (InitClient definition in definitions)
        {
            if (existing.TryGetValue(definition.ClientId, out Client? client))
            {
                result.Skipped++;
                if (options.OverwriteExisting)
                    Apply(client, definition);
                else
                    CheckClient(client, definition, result);
                continue;
            }

            db.Clients.Add(Create(definition));
            result.Inserted++;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// 按 IdentityServer 对 <c>SharedSecret</c> 的默认约定计算密钥哈希：SHA-256 摘要的 Base64。
    /// </summary>
    /// <param name="plainText">密钥明文。</param>
    /// <returns>入库使用的哈希值。</returns>
    /// <remarks>
    /// IdentityServer 校验 <c>type = SharedSecret</c> 的密钥时，就是对明文做同样的摘要后比对，
    /// 因此这里必须与之一致。<c>InitDataHashTest</c> 用历史数据库中已有的哈希值把这一约定钉死。
    /// </remarks>
    public static string HashSecret(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(plainText)));
    }

    private static async Task<InitDataResult> EnsureCoreAsync(
        ConfigurationDbContext db,
        InitDataEnsureOptions? options,
        CancellationToken cancellationToken)
    {
        InitDataResult result = new();
        result.Absorb(await EnsureApiScopesAsync(db, null, options, cancellationToken).ConfigureAwait(false));
        result.Absorb(await EnsureIdentityResourcesAsync(db, null, options, cancellationToken).ConfigureAwait(false));
        result.Absorb(await EnsureClientsAsync(db, null, options, cancellationToken).ConfigureAwait(false));
        return result;
    }

    private static ApiScope Create(InitApiScope definition)
    {
        ApiScope scope = new();
        Apply(scope, definition);
        return scope;
    }

    private static void Apply(ApiScope scope, InitApiScope definition)
    {
        scope.Name = definition.Name;
        scope.DisplayName = definition.DisplayName;
        scope.Description = definition.Description;
        scope.Created = definition.Created;
        // 历史值为 Created（原因见 Apply(Client) 中的说明）。
        scope.Updated = definition.Created;
        scope.Required = definition.Required;
        scope.Emphasize = definition.Emphasize;
        scope.Enabled = true;
        scope.ShowInDiscoveryDocument = true;
        scope.NonEditable = true;
    }

    private static IdentityResource Create(InitIdentityResource definition)
    {
        IdentityResource resource = new();
        Apply(resource, definition);
        return resource;
    }

    private static void Apply(IdentityResource resource, InitIdentityResource definition)
    {
        resource.Name = definition.Name;
        resource.DisplayName = definition.DisplayName;
        resource.Description = definition.Description;
        resource.Created = definition.Created;
        // 历史值为 Created（原因见 Apply(Client) 中的说明）。
        resource.Updated = definition.Created;
        resource.Required = definition.Required;
        resource.Emphasize = definition.Emphasize;
        resource.Enabled = true;
        resource.ShowInDiscoveryDocument = true;
        resource.NonEditable = true;

        // Duende 的实体类不会初始化导航集合，new 出来的实例上取到的是 null。
        resource.UserClaims ??= [];
        Replace(resource.UserClaims, definition.UserClaims, claim => new IdentityResourceClaim { Type = claim });
    }

    private static Client Create(InitClient definition)
    {
        Client client = new();
        Apply(client, definition);
        return client;
    }

    private static void Apply(Client client, InitClient definition)
    {
        client.ClientId = definition.ClientId;
        client.ClientName = definition.ClientName;
        client.Description = definition.Description;
        client.Created = definition.Created;
        client.Enabled = true;
        client.NonEditable = true;
        client.AllowOfflineAccess = definition.AllowOfflineAccess;
        // 刻意不动 client.Updated：历史值在 Clients 上为 NULL，而在 ApiScopes / IdentityResources 上
        // 等于 Created。这处不一致照抄自历史脚本，目的是让新旧种子灌出的库逐字段一致。
        // Created / Updated / LastAccessed / NonEditable 属于 IdentityServer 的簿记字段，
        // 不参与下面的差异检查。

        // 实体默认值为 Custom，而历史脚本为内置客户端写入了 Iat。
        client.DPoPValidationMode = DPoPValidationMode.Iat;

        // Duende 的实体类不会初始化导航集合，new 出来的实例上取到的是 null。
        client.AllowedGrantTypes ??= [];
        client.AllowedScopes ??= [];
        client.AllowedCorsOrigins ??= [];
        client.RedirectUris ??= [];
        client.PostLogoutRedirectUris ??= [];
        client.ClientSecrets ??= [];

        Replace(client.AllowedGrantTypes, definition.GrantTypes, grantType => new ClientGrantType { GrantType = grantType });
        Replace(client.AllowedScopes, definition.Scopes, scope => new ClientScope { Scope = scope });
        Replace(client.AllowedCorsOrigins, definition.CorsOrigins, origin => new ClientCorsOrigin { Origin = origin });
        Replace(client.RedirectUris, definition.RedirectUris, uri => new ClientRedirectUri { RedirectUri = uri });
        Replace(client.PostLogoutRedirectUris, definition.PostLogoutRedirectUris, uri => new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = uri });

        client.ClientSecrets.Clear();
        if (definition.Secret is not null)
        {
            client.ClientSecrets.Add(new ClientSecret
            {
                Value = HashSecret(definition.Secret.PlainText),
                Created = definition.Secret.Created,
            });
        }
    }

    private static void Replace<T>(ICollection<T> target, IReadOnlyList<string>? values, Func<string, T> factory)
    {
        target.Clear();
        if (values is null) return;
        foreach (string value in values) target.Add(factory(value));
    }

    private static void CheckApiScope(ApiScope scope, InitApiScope definition, InitDataResult result)
    {
        const string kind = "API 范围";
        Check(result, kind, scope.Name, "显示名称", scope.DisplayName, definition.DisplayName);
        Check(result, kind, scope.Name, "说明", scope.Description, definition.Description);
        Check(result, kind, scope.Name, "Required", scope.Required, definition.Required);
        Check(result, kind, scope.Name, "Emphasize", scope.Emphasize, definition.Emphasize);
    }

    private static void CheckIdentityResource(IdentityResource resource, InitIdentityResource definition, InitDataResult result)
    {
        const string kind = "标识资源";
        Check(result, kind, resource.Name, "显示名称", resource.DisplayName, definition.DisplayName);
        Check(result, kind, resource.Name, "说明", resource.Description, definition.Description);
        Check(result, kind, resource.Name, "Required", resource.Required, definition.Required);
        Check(result, kind, resource.Name, "Emphasize", resource.Emphasize, definition.Emphasize);
        CheckSet(result, kind, resource.Name, "签发的声明", resource.UserClaims.Select(claim => claim.Type), definition.UserClaims);
    }

    private static void CheckClient(Client client, InitClient definition, InitDataResult result)
    {
        const string kind = "客户端";
        Check(result, kind, client.ClientId, "名称", client.ClientName, definition.ClientName);
        Check(result, kind, client.ClientId, "说明", client.Description, definition.Description);
        Check(result, kind, client.ClientId, "是否允许离线访问", client.AllowOfflineAccess, definition.AllowOfflineAccess);
        CheckSet(result, kind, client.ClientId, "授权类型", client.AllowedGrantTypes.Select(item => item.GrantType), definition.GrantTypes);
        CheckSet(result, kind, client.ClientId, "范围", client.AllowedScopes.Select(item => item.Scope), definition.Scopes);
        CheckSet(result, kind, client.ClientId, "跨域来源", client.AllowedCorsOrigins.Select(item => item.Origin), definition.CorsOrigins);
        CheckSet(result, kind, client.ClientId, "重定向地址", client.RedirectUris.Select(item => item.RedirectUri), definition.RedirectUris);
        CheckSet(result, kind, client.ClientId, "注销后重定向地址", client.PostLogoutRedirectUris.Select(item => item.PostLogoutRedirectUri), definition.PostLogoutRedirectUris);

        // 密钥只比对「定义中的明文派生出的哈希」是否仍在库中，不比对哈希本身是否相同：
        // 这样既能发现密钥已被轮换（而配置没跟着改），又不会把哈希泄漏进日志。
        if (definition.Secret is null) return;
        string expected = HashSecret(definition.Secret.PlainText);
        if (!client.ClientSecrets.Any(secret => string.Equals(secret.Value, expected, StringComparison.Ordinal)))
            result.Warn($"{kind} 内置数据「{client.ClientId}」的密钥与仓库定义不一致：数据库中不存在由定义明文派生出的哈希，可能已在数据库中被轮换，或配置文件中的 ClientSecret 与内置数据不再匹配。");
    }

    private static void Check(InitDataResult result, string kind, string key, string field, object? actual, object? expected)
    {
        if (Equals(Format(actual), Format(expected))) return;
        result.Warn($"{kind} 内置数据「{key}」的 {field} 与仓库定义不一致：数据库中为 {Format(actual)}，仓库定义为 {Format(expected)}。");
    }

    private static void CheckSet(InitDataResult result, string kind, string key, string field, IEnumerable<string> actual, IReadOnlyList<string>? expected)
    {
        HashSet<string> actualSet = [.. actual];
        HashSet<string> expectedSet = [.. expected ?? []];
        if (actualSet.SetEquals(expectedSet)) return;
        result.Warn($"{kind} 内置数据「{key}」的 {field} 与仓库定义不一致：数据库中为 [{string.Join(", ", actualSet.Order(StringComparer.Ordinal))}]，仓库定义为 [{string.Join(", ", expectedSet.Order(StringComparer.Ordinal))}]。");
    }

    private static string Format(object? value) => value switch
    {
        null => "NULL",
        bool flag => flag ? "true" : "false",
        _ => value.ToString() ?? "NULL",
    };
}
