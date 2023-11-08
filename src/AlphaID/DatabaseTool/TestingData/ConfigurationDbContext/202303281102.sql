SET IDENTITY_INSERT [dbo].[ApiResources] ON 
SET IDENTITY_INSERT [dbo].[ApiResources] OFF
SET IDENTITY_INSERT [dbo].[ApiResourceClaims] ON 
SET IDENTITY_INSERT [dbo].[ApiResourceClaims] OFF
SET IDENTITY_INSERT [dbo].[ApiResourceScopes] ON 
SET IDENTITY_INSERT [dbo].[ApiResourceScopes] OFF
SET IDENTITY_INSERT [dbo].[ApiScopes] ON 
INSERT [dbo].[ApiScopes] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [LastAccessed], [NonEditable]) VALUES (3, 1, N'read', N'读取', NULL, 0, 0, 1, CAST(N'2022-12-27T10:52:00.0000000' AS DateTime2), NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[ApiScopes] OFF
SET IDENTITY_INSERT [dbo].[Clients] ON 
INSERT [dbo].[Clients] ([Id], [Enabled], [ClientId], [ProtocolType], [RequireClientSecret], [ClientName], [Description], [ClientUri], [LogoUri], [RequireConsent], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [RequirePkce], [AllowPlainTextPkce], [RequireRequestObject], [AllowAccessTokensViaBrowser], [RequireDPoP], [DPoPValidationMode], [DPoPClockSkew], [FrontChannelLogoutUri], [FrontChannelLogoutSessionRequired], [BackChannelLogoutUri], [BackChannelLogoutSessionRequired], [AllowOfflineAccess], [IdentityTokenLifetime], [AllowedIdentityTokenSigningAlgorithms], [AccessTokenLifetime], [AuthorizationCodeLifetime], [ConsentLifetime], [AbsoluteRefreshTokenLifetime], [SlidingRefreshTokenLifetime], [RefreshTokenUsage], [UpdateAccessTokenClaimsOnRefresh], [RefreshTokenExpiration], [AccessTokenType], [EnableLocalLogin], [IncludeJwtId], [AlwaysSendClientClaims], [ClientClaimsPrefix], [PairWiseSubjectSalt], [UserSsoLifetime], [UserCodeType], [DeviceCodeLifetime], [CibaLifetime], [PollingInterval], [CoordinateLifetimeWithUserSession], [Created], [Updated], [LastAccessed], [NonEditable]) VALUES (4, 1, N'd70700eb-c4d8-4742-a79a-6ecf2064b27c', N'oidc', 1, N'Alpha ID Management Center', 'Used for current web application.', NULL, NULL, 0, 1, 0, 1, 0, 0, 0, 0, 1, CONVERT(DATETIME, '2023-10-03 00:05:00', 102), NULL, 1, NULL, 1, 0, 300, NULL, 3600, 300, NULL, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, N'client_', NULL, NULL, NULL, 300, NULL, NULL, NULL, CAST(N'2022-12-23T20:37:00.0000000' AS DateTime2), NULL, NULL, 1)
INSERT [dbo].[Clients] ([Id], [Enabled], [ClientId], [ProtocolType], [RequireClientSecret], [ClientName], [Description], [ClientUri], [LogoUri], [RequireConsent], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [RequirePkce], [AllowPlainTextPkce], [RequireRequestObject], [AllowAccessTokensViaBrowser], [RequireDPoP], [DPoPValidationMode], [DPoPClockSkew], [FrontChannelLogoutUri], [FrontChannelLogoutSessionRequired], [BackChannelLogoutUri], [BackChannelLogoutSessionRequired], [AllowOfflineAccess], [IdentityTokenLifetime], [AllowedIdentityTokenSigningAlgorithms], [AccessTokenLifetime], [AuthorizationCodeLifetime], [ConsentLifetime], [AbsoluteRefreshTokenLifetime], [SlidingRefreshTokenLifetime], [RefreshTokenUsage], [UpdateAccessTokenClaimsOnRefresh], [RefreshTokenExpiration], [AccessTokenType], [EnableLocalLogin], [IncludeJwtId], [AlwaysSendClientClaims], [ClientClaimsPrefix], [PairWiseSubjectSalt], [UserSsoLifetime], [UserCodeType], [DeviceCodeLifetime], [CibaLifetime], [PollingInterval], [CoordinateLifetimeWithUserSession], [Created], [Updated], [LastAccessed], [NonEditable]) VALUES (14, 1, N'5aa8bed6-4f57-47ac-82e9-08b5874c64e3', N'oidc', 1, N'Alpha ID WebAPI Document Protection', NULL, NULL, NULL, 0, 1, 0, 1, 0, 0, 0, 0, 1, CONVERT(DATETIME, '2023-10-03 00:05:00', 102), NULL, 0, NULL, 0, 0, 300, NULL, 3600, 300, NULL, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, N'client_', NULL, NULL, NULL, 300, NULL, NULL, NULL, CAST(N'2023-03-24T08:59:55.6460028' AS DateTime2), CAST(N'2023-03-24T08:59:55.6460028' AS DateTime2), NULL, 0)
SET IDENTITY_INSERT [dbo].[Clients] OFF
SET IDENTITY_INSERT [dbo].[ClientGrantTypes] ON 
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (14, N'authorization_code', 4)
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (9, N'client_credentials', 4)
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (10, N'password', 4)
INSERT [dbo].[ClientGrantTypes] ([Id], [GrantType], [ClientId]) VALUES (23, N'authorization_code', 14)
SET IDENTITY_INSERT [dbo].[ClientGrantTypes] OFF
SET IDENTITY_INSERT [dbo].[ClientPostLogoutRedirectUris] ON 
INSERT [dbo].[ClientPostLogoutRedirectUris] ([Id], [PostLogoutRedirectUri], [ClientId]) VALUES (2, N'https://localhost:61315/signout-callback-oidc', 4)
SET IDENTITY_INSERT [dbo].[ClientPostLogoutRedirectUris] OFF
SET IDENTITY_INSERT [dbo].[ClientRedirectUris] ON 
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (8, N'https://A-200094071.changingsoft.com:44355/signin-oidc', 4)
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (2, N'https://localhost:61315/signin-oidc', 4)
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (5, N'https://oauth.pstmn.io/v1/callback', 4)
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (21, N'https://localhost:61316/signin-oidc', 14)
INSERT [dbo].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId]) VALUES (22, N'https://localhost:61316/docs/oauth2-redirect.html', 14)
SET IDENTITY_INSERT [dbo].[ClientRedirectUris] OFF
SET IDENTITY_INSERT [dbo].[ClientScopes] ON 
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (5, N'openid', 4)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (6, N'profile', 4)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (11, N'realname', 4)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (7, N'user_impersonation', 4)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (45, N'openid', 14)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (46, N'profile', 14)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (47, N'user_impersonation', 14)
INSERT [dbo].[ClientScopes] ([Id], [Scope], [ClientId]) VALUES (48, N'realname', 14)
SET IDENTITY_INSERT [dbo].[ClientScopes] OFF
SET IDENTITY_INSERT [dbo].[ClientSecrets] ON 
INSERT [dbo].[ClientSecrets] ([Id], [ClientId], [Description], [Value], [Expiration], [Type], [Created]) VALUES (3, 4, NULL, N'bpHsoigKdYWDrsxWC2qSXbUViZMkA0xg72mcf9ktD1M=', NULL, N'SharedSecret', CAST(N'2022-12-23T20:39:00.0000000' AS DateTime2))
INSERT [dbo].[ClientSecrets] ([Id], [ClientId], [Description], [Value], [Expiration], [Type], [Created]) VALUES (9, 14, NULL, N'CsdIN2hTdHvy3tIdxWPn8UoU1etGToS8NdAgPTS5Nao=', NULL, N'SharedSecret', CAST(N'2023-03-24T08:59:55.6460028' AS DateTime2))
SET IDENTITY_INSERT [dbo].[ClientSecrets] OFF
SET IDENTITY_INSERT [dbo].[ClientCorsOrigins] ON
INSERT INTO [dbo].[ClientCorsOrigins] ([Id], [Origin], [ClientId]) VALUES (1, 'https://localhost:61316', 14)
SET IDENTITY_INSERT [dbo].[ClientCorsOrigins] OFF
