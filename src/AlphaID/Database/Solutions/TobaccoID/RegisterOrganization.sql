USE TobaccoIDData;
GO
--机构全称
DECLARE @Name nvarchar(50) = N'';
--住所
DECLARE @Domicile nvarchar(100) = N'';
--联系方式
DECLARE @Contact varchar(50) = N'';
--成立时间
DECLARE @WhenRegistered date = '';
--营业期限
DECLARE @Expires date = '';
--法定代表人名称
DECLARE @LegalPersonName nvarchar = N'';
--统一社会信用代码
DECLARE @USCI varchar(18) = '';


DECLARE @Now datetime2(7) = GETDATE();
DECLARE @Id varchar(128) = NEWID();

BEGIN TRAN

BEGIN TRY

IF @Name = N'' THROW 54001, 'Organization name required.', 1;

IF EXISTS(select Name
from Subject inner join Organization on Subject.Id = Organization.Id
where Name = @Name)
BEGIN
    SELECT Subject.Id, Subject.Name
    FROM Subject inner join Organization on Subject.Id = Organization.Id
    where Name = @Name;
    THROW 54001, '该机构已存在', 1;
END;

IF EXISTS(select IdentifierValue
from SubjectIdentity
where IdentifierValue = @USCI)
THROW 54001, '统一社会信用代码已被登记', 1;

INSERT INTO Subject
    (Id, Name, Domicile, Contact, WhenCreated, WhenChanged, Enabled)
VALUES
    (@Id, @Name, @Domicile, @Contact, @Now, @Now, 1);

INSERT INTO Organization
    (Id, WhenRegistered, Expires, LegalPersonName)
VALUES
    (@Id, @WhenRegistered, @Expires, @LegalPersonName);

INSERT INTO SubjectIdentity
    (SubjectId, IdentifierType, IdentifierValue, WhenUpdated)
VALUES
    (@Id, N'统一社会信用代码', @USCI, @Now);

INSERT INTO BusinessLicense
    (IdentifierType, IdentifierValue, WhenRegistered)
VALUES
    (N'统一社会信用代码', @USCI, @WhenRegistered);

	COMMIT TRAN

	PRINT '已成功注册组织：'+ @Id;

END TRY
BEGIN CATCH
	ROLLBACK TRAN;
	THROW;
END CATCH;