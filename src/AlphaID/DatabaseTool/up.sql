BEGIN TRANSACTION;
GO

ALTER TABLE [NaturalPerson] ADD [DateOfBirth] date NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230518070125_AddColumns', N'7.0.5');
GO

COMMIT;
GO

