# Init Data

Alpha ID 正常工作需要一些内置数据。没有系统工具支持编辑和删除这些数据，但你可以通过访问数据库来编辑他们。

**请注意，内置数据对系统运行至关重要！您必须充分了解该系统并做好备份回退工作后，再编辑这些内置数据。**

## 内置数据在哪里

内置数据（IdentityServer 的客户端、API 范围、标识资源）以 C# 常量的形式定义在 `AlphaId.InitData` 项目中：

|文件|内容|
|---|---|
|`InitData.Clients.cs`|客户端（`Clients` 表）|
|`InitData.ApiScopes.cs`|API 范围（`ApiScopes` 表）|
|`InitData.IdentityResources.cs`|标识资源（`IdentityResources` 表）|
|`InitDataSeeder.cs`|负责把上述数据幂等地写入数据库|

写入由两个入口触发：

- 运行 `DatabaseTool` 时，在 `AddInitData` 阶段写入（`AddInitData` 默认为 `true`）。
- 运行集成测试时，测试宿主通过 `InitDataTestExtensions.EnsureInitData()` 自行补齐。测试不再依赖「数据库碰巧被 `DatabaseTool` 灌过」。

两个入口都是**幂等**的：以自然键（`Client.ClientId`、`ApiScope.Name`、`IdentityResource.Name`）查找，缺失则插入，已存在则跳过。
已存在但与代码不一致的行只会产生告警，不会被覆盖，除非显式打开 `OverwriteInitData`。

因此**若要修改内置数据，请改 `AlphaId.InitData` 里的常量，然后重新运行 `DatabaseTool`**，而不要直接改数据库：
直接改数据库不会同步到其他环境，而且会在下次运行 `DatabaseTool` 时被报为「与代码不一致」。

## 客户端

|Name|Client ID|Grant Types|PKCE|Redirect URIs|Scopes|Description|
|---|---|---|---|---|---|---|
|AlphaID Management Center|d70700eb-c4d8-4742-a79a-6ecf2064b27c|authorization_code, client_credentials, password|是|https://localhost:49728/signin-oidc|openid, profile, public|Used for current web application.|
|AlphaID AuthCenter Swagger UI|43670b09-b161-46ca-b59a-c0fbde526394|authorization_code|是|https://localhost:49726/api-docs/oauth2-redirect.html、https://oauth.pstmn.io/v1/callback|openid, profile, membership|AuthCenter 的 Swagger UI 使用。|

客户端的 `ClientSecret` 以 `base64(SHA256(明文))` 形式存储。两处明文分别配置在
`AdminWebApp/appsettings.json` 的 `OidcClient` 节与 `AuthCenterWebApp/appsettings.Development.json` 的 `Swagger` 节。

> `AlphaIdWebAPI` 使用的客户端（`5aa8bed6-4f57-47ac-82e9-08b5874c64e3`）**不属于内置数据**：
> 它从未出现在历史脚本中，而是通过管理界面创建的，属于「部署时配置」。

## Scopes

系统内置的Scope：

|Name|Display Name|Description|Required|Emphasize|Show in disco|
|---|---|---|---|---|---|
|public|公共API|可访问受保护的公共API|否|否|是|
|realname|实名信息|获取自然人的实名制信息，如身份证号码|否|是|是|
|membership|组织成员|获取用户的组织成员身份|否|是|是|

## Identity Resources

系统内置的标识资源：

|Name|Display Name|Description|Required|Emphasize|Show in disco|Issue Claims|
|---|---|---|---|---|---|---|
|openid|您的用户标识符|您的Id|是|否|是|sub|
|profile|用户配置文件|您的基本信息，如姓名等|否|是|是|birthdate, family_name, gender, given_name, locale, middle_name, name, nickname, picture, preferred_username, profile, search_hint, updated_at, website, zoneinfo|
|email|您的电子邮件地址|您的电子邮件地址|否|是|是|email, email_verified|
|address|您的邮政地址|您的邮政地址|否|是|是|address|
|phone|您的手机号|您的手机号|否|是|是|phone_number, phone_number_verified|
