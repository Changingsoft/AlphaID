USE TobaccoIDData;
GO

--身份证号
DECLARE @ChineseIDCardNumber varchar(50) = '';
--姓氏
DECLARE @LastName nvarchar(50) = '';
--名字
DECLARE @FirstName nvarchar(50) = '';
--姓名
DECLARE @Name nvarchar(50) = '';
--拼音名称
DECLARE @PhoneticDisplayName varchar(50) = '';
--性别 Male Female Other
DECLARE @Sex varchar(50) = 'Male';
--出生日期
DECLARE @DateOfBirth date = '1994-8-6';
--民族
DECLARE @Ethnicity nvarchar(50) = '';
--住址
DECLARE @Address nvarchar(100) = '';
--签发机关
DECLARE @Issuer nvarchar(100) = '';
--签发日期
DECLARE @IssueDate date = '1900-1-1';
--有效期至
DECLARE @Expires date = '1900-1-1';

--手机号码 不加国际电话前缀如 13512345678
DECLARE @Mobile varchar(50) = '';
--邮件地址
DECLARE @Email varchar(100) = '';

--登录账号标识符
DECLARE @LogonId varchar = '';
--User Principal Name
DECLARE @UPN varchar = '';

--所属组织
DECLARE @OrganizationId varchar(128) = '';
--部门名称
DECLARE @Department nvarchar(50) = N'';
--职务
DECLARE @Title nvarchar(50) = N'';

DECLARE @Id varchar(128) = NEWID();
DECLARE @Now datetime2(7) = GETDATE();

BEGIN TRAN

BEGIN TRY

IF EXISTS(select *
from ChineseIDCardIdentity
where IdentifierValue = @ChineseIDCardNumber)
BEGIN
    SELECT Subject.*
    FROM Subject inner join SubjectIdentity on Subject.Id = SubjectIdentity.SubjectId
    where SubjectIdentity.IdentifierValue = @ChineseIDCardNumber;
    THROW 51000, 'Person Exists.',1;
END

INSERT INTO Subject
    (Id, Name, WhenCreated, WhenChanged, Enabled)
VALUES
    (@Id, @Name, @Now, @Now, 1);

INSERT INTO Person
    (Id, FirstName, LastName, PhoneticDisplayName, Sex, Ethnicity, Mobile, Email)
VALUES
    (@Id, @FirstName, @LastName, @PhoneticDisplayName, @Sex, @Ethnicity, '+86'+ @Mobile, @Email);

INSERT INTO SubjectIdentity
    (SubjectId, IdentifierType, IdentifierValue, WhenUpdated)
VALUES
    (@Id, N'身份证', @ChineseIDCardNumber, @Now);

INSERT INTO ChineseIDCardIdentity
    (IdentifierType, IdentifierValue, Name, Sex, Ethnicity, DateOfBirth, Address, Issuer, IssueDate, Expires)
VALUES
    (N'身份证', @ChineseIDCardNumber, @Name, @Sex, @Ethnicity, @DateOfBirth, @Address, @Issuer, @IssueDate, @Expires);

INSERT INTO LogonAccount
    (LogonId, PersonId, IdentityProvider, UserPrincipalName)
VALUES
    (@LogonId, @Id, N'LDAP://ynyc.com', @UPN);

INSERT INTO OrganizationMember
    (OrganizationId, PersonId, Department, Title)
VALUES
    (@OrganizationId, @Id, @Department, @Title);

	COMMIT TRAN;

	PRINT '已成功注册用户：' + @Id;

END TRY
BEGIN CATCH
	ROLLBACK TRAN;
	THROW;
END CATCH



