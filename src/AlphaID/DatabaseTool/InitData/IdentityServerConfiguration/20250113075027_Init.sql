SET IDENTITY_INSERT [dbo].[ApiScopes] ON 
INSERT [dbo].[ApiScopes] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [LastAccessed], [NonEditable]) VALUES (1, 1, N'user_impersonation', N'模拟个人身份', N'为应用程序请求使用已登录用户的身份访问资源的权限。', 0, 0, 1, CAST(N'2023-02-10T13:22:00.0000000' AS DateTime2), CAST(N'2023-02-10T13:22:00.0000000' AS DateTime2), NULL, 1)
INSERT [dbo].[ApiScopes] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [LastAccessed], [NonEditable]) VALUES (2, 1, N'realname', N'实名信息', N'获取自然人的实名制信息，如身份证号码', 0, 1, 1, CAST(N'2023-02-08T14:32:00.0000000' AS DateTime2), CAST(N'2023-02-08T14:32:00.0000000' AS DateTime2), NULL, 1)
INSERT [dbo].[ApiScopes] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [LastAccessed], [NonEditable]) VALUES (3, 1, N'membership', N'组织成员', N'获取用户的组织成员身份', 0, 1, 1, CAST(N'2023-02-08T14:32:00.0000000' AS DateTime2), CAST(N'2023-02-08T14:32:00.0000000' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[ApiScopes] OFF
SET IDENTITY_INSERT [dbo].[IdentityResources] ON 
INSERT [dbo].[IdentityResources] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [NonEditable]) VALUES (1, 1, N'openid', N'您的用户标识符', N'您的Id', 1, 0, 1, CAST(N'2022-12-15T20:27:25.2378862' AS DateTime2), CAST(N'2022-12-15T20:27:25.2378862' AS DateTime2), 1)
INSERT [dbo].[IdentityResources] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [NonEditable]) VALUES (2, 1, N'profile', N'用户配置文件', N'您的基本信息，如姓名等', 0, 1, 1, CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), 1)
INSERT [dbo].[IdentityResources] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [NonEditable]) VALUES (3, 1, N'email', N'您的电子邮件地址', N'您的电子邮件地址', 0, 1, 1, CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), 1)
INSERT [dbo].[IdentityResources] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [NonEditable]) VALUES (4, 1, N'address', N'您的邮政地址', N'您的邮政地址', 0, 1, 1, CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), 1)
INSERT [dbo].[IdentityResources] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [NonEditable]) VALUES (5, 1, N'phone', N'您的手机号', N'您的手机号', 0, 1, 1, CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), CAST(N'2022-12-15T20:27:25.2681944' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[IdentityResources] OFF
SET IDENTITY_INSERT [dbo].[IdentityResourceClaims] ON 
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (1, 1, N'sub')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (2, 2, N'name')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (3, 2, N'family_name')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (4, 2, N'given_name')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (5, 2, N'middle_name')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (6, 2, N'nickname')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (7, 2, N'preferred_username')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (8, 2, N'profile')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (9, 2, N'picture')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (10, 2, N'website')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (11, 2, N'gender')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (12, 2, N'birthdate')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (13, 2, N'zoneinfo')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (14, 2, N'locale')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (15, 2, N'updated_at')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (16, 2, N'search_hint')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (17, 3, N'email')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (18, 3, N'email_verified')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (19, 4, N'address')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (20, 5, N'phone_number')
INSERT [dbo].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (21, 5, N'phone_number_verified')
SET IDENTITY_INSERT [dbo].[IdentityResourceClaims] OFF
SET IDENTITY_INSERT [dbo].[IdentityResourceProperties] ON 
SET IDENTITY_INSERT [dbo].[IdentityResourceProperties] OFF
SET IDENTITY_INSERT [dbo].[IdentityProviders] ON 
SET IDENTITY_INSERT [dbo].[IdentityProviders] OFF
SET IDENTITY_INSERT [dbo].[ApiResources] ON 
SET IDENTITY_INSERT [dbo].[ApiResources] OFF
SET IDENTITY_INSERT [dbo].[ApiResourceClaims] ON 
SET IDENTITY_INSERT [dbo].[ApiResourceClaims] OFF
SET IDENTITY_INSERT [dbo].[ApiResourceScopes] ON 
SET IDENTITY_INSERT [dbo].[ApiResourceScopes] OFF
SET IDENTITY_INSERT [dbo].[Clients] ON 
INSERT [dbo].[Clients] ([Id], [Enabled], [ClientId], [ProtocolType], [RequireClientSecret], [ClientName], [Description], [ClientUri], [LogoUri], [RequireConsent], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [RequirePkce], [AllowPlainTextPkce], [RequireRequestObject], [AllowAccessTokensViaBrowser], [RequireDPoP], [DPoPValidationMode], [DPoPClockSkew], [FrontChannelLogoutUri], [FrontChannelLogoutSessionRequired], [BackChannelLogoutUri], [BackChannelLogoutSessionRequired], [AllowOfflineAccess], [IdentityTokenLifetime], [AllowedIdentityTokenSigningAlgorithms], [AccessTokenLifetime], [AuthorizationCodeLifetime], [ConsentLifetime], [AbsoluteRefreshTokenLifetime], [SlidingRefreshTokenLifetime], [RefreshTokenUsage], [UpdateAccessTokenClaimsOnRefresh], [RefreshTokenExpiration], [AccessTokenType], [EnableLocalLogin], [IncludeJwtId], [AlwaysSendClientClaims], [ClientClaimsPrefix], [PairWiseSubjectSalt], [UserSsoLifetime], [UserCodeType], [DeviceCodeLifetime], [CibaLifetime], [PollingInterval], [CoordinateLifetimeWithUserSession], [Created], [Updated], [LastAccessed], [NonEditable], [PushedAuthorizationLifetime], [RequirePushedAuthorization]) VALUES (4, 1, N'd70700eb-c4d8-4742-a79a-6ecf2064b27c', N'oidc', 1, N'AlphaID Management Center', 'Used for current web application.', NULL, NULL, 0, 1, 0, 1, 0, 0, 0, 0, 1, CONVERT(DATETIME, '2023-10-03 00:05:00', 102), NULL, 1, NULL, 1, 0, 300, NULL, 3600, 300, NULL, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, N'client_', NULL, NULL, NULL, 300, NULL, NULL, NULL, CAST(N'2022-12-23T20:37:00.0000000' AS DateTime2), NULL, NULL, 1, NULL, 0)
INSERT [dbo].[Clients] ([Id], [Enabled], [ClientId], [ProtocolType], [RequireClientSecret], [ClientName], [Description], [ClientUri], [LogoUri], [RequireConsent], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [RequirePkce], [AllowPlainTextPkce], [RequireRequestObject], [AllowAccessTokensViaBrowser], [RequireDPoP], [DPoPValidationMode], [DPoPClockSkew], [FrontChannelLogoutUri], [FrontChannelLogoutSessionRequired], [BackChannelLogoutUri], [BackChannelLogoutSessionRequired], [AllowOfflineAccess], [IdentityTokenLifetime], [AllowedIdentityTokenSigningAlgorithms], [AccessTokenLifetime], [AuthorizationCodeLifetime], [ConsentLifetime], [AbsoluteRefreshTokenLifetime], [SlidingRefreshTokenLifetime], [RefreshTokenUsage], [UpdateAccessTokenClaimsOnRefresh], [RefreshTokenExpiration], [AccessTokenType], [EnableLocalLogin], [IncludeJwtId], [AlwaysSendClientClaims], [ClientClaimsPrefix], [PairWiseSubjectSalt], [UserSsoLifetime], [UserCodeType], [DeviceCodeLifetime], [CibaLifetime], [PollingInterval], [CoordinateLifetimeWithUserSession], [Created], [Updated], [LastAccessed], [NonEditable], [PushedAuthorizationLifetime], [RequirePushedAuthorization]) VALUES (5, 1, N'43670b09-b161-46ca-b59a-c0fbde526394', N'oidc', 1, N'AlphaID AuthCenter Swagger UI', NULL, NULL, NULL, 0, 1, 0, 1, 0, 0, 0, 0, 1, CONVERT(DATETIME, '2023-10-03 00:05:00', 102), NULL, 1, NULL, 1, 0, 300, NULL, 3600, 300, NULL, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, N'client_', NULL, NULL, NULL, 300, NULL, NULL, NULL, CAST(N'2022-12-23T20:37:00.0000000' AS DateTime2), NULL, NULL, 1, NULL, 0)
SET IDENTITY_INSERT [dbo].[Clients] OFF
SET IDENTITY_INSERT [dbo].[ClientGrantTypes] ON 
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (1, N'authorization_code', 4)
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (2, N'client_credentials', 4)
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (3, N'password', 4)
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (4, N'authorization_code', 5)
SET IDENTITY_INSERT [dbo].[ClientGrantTypes] OFF
SET IDENTITY_INSERT [dbo].[ClientPostLogoutRedirectUris] ON 
INSERT [dbo].[ClientPostLogoutRedirectUris] ([Id], [PostLogoutRedirectUri], [ClientId]) VALUES (1, N'https://localhost:49728/signout-callback-oidc', 4)
SET IDENTITY_INSERT [dbo].[ClientPostLogoutRedirectUris] OFF
SET IDENTITY_INSERT [dbo].[ClientRedirectUris] ON 
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (1, N'https://localhost:49728/signin-oidc', 4)
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (2, N'https://localhost:49727/docs/oauth2-redirect.html', 4)
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (3, N'https://localhost:49726/api-docs/oauth2-redirect.html', 5)
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (4, N'https://oauth.pstmn.io/v1/callback', 5)
SET IDENTITY_INSERT [dbo].[ClientRedirectUris] OFF
SET IDENTITY_INSERT [dbo].[ClientScopes] ON 
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (1, N'openid', 4)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (3, N'profile', 4)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (4, N'user_impersonation', 4)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (5, N'openid', 5)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (6, N'profile', 5)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (7, N'membership', 5)
SET IDENTITY_INSERT [dbo].[ClientScopes] OFF
SET IDENTITY_INSERT [dbo].[ClientSecrets] ON 
INSERT [dbo].[ClientSecrets] ([Id], [ClientId], [Description], [Value], [Expiration], [Type], [Created]) VALUES (1, 4, NULL, N'bpHsoigKdYWDrsxWC2qSXbUViZMkA0xg72mcf9ktD1M=', NULL, N'SharedSecret', CAST(N'2022-12-23T20:39:00.0000000' AS DateTime2))
INSERT [dbo].[ClientSecrets] ([Id], [ClientId], [Description], [Value], [Expiration], [Type], [Created]) VALUES (2, 5, NULL, N'illTewgHf1df4qb1JsQtd5u7BhPvOkG+PSpPcJIf7aY=', NULL, N'SharedSecret', CAST(N'2022-12-23T20:39:00.0000000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[ClientSecrets] OFF
SET IDENTITY_INSERT [dbo].[ClientCorsOrigins] ON
INSERT [dbo].[ClientCorsOrigins] ([Id], [Origin], [ClientId]) VALUES (1, N'https://localhost:49727', 4)
SET IDENTITY_INSERT [dbo].[ClientCorsOrigins] OFF
SET IDENTITY_INSERT [dbo].[ClientIdPRestrictions] ON
SET IDENTITY_INSERT [dbo].[ClientIdPRestrictions] OFF
SET IDENTITY_INSERT [dbo].[ClientProperties] ON
SET IDENTITY_INSERT [dbo].[ClientProperties] OFF
