USE [AlphaIDData-62c1d690-6157-4b72-b592-86ffc498c66f]
GO
SET IDENTITY_INSERT [dbo].[DirectoryService] ON 
GO
INSERT [dbo].[DirectoryService] ([Id], [Name], [Type], [ServerAddress], [RootDn], [DefaultUserAccountContainer], [UserName], [Password], [UpnSuffix], [SamDomainPart], [AutoCreateAccount]) VALUES (2, N'ADLDS', N'ADLDS', N'localhost', N'DC=changingsoft,DC=com', N'DC=changingsoft,DC=com', NULL, NULL, N'changingsoft.com', NULL, 0)
GO
SET IDENTITY_INSERT [dbo].[DirectoryService] OFF
GO
INSERT [dbo].[LogonAccount] ([ObjectId], [ServiceId], [UserId]) VALUES (N'1e0e2b35-a42a-4c0d-be87-266c02ab20a4', 2, N'd2480421-8a15-4292-8e8f-06985a1f645b')
GO
