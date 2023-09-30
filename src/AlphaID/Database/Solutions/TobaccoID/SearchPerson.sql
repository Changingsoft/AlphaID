USE TobaccoIDData
GO
--使用姓名、全拼、手机号或身份证号码检索自然人。
DECLARE @Keywords nvarchar(10) = N'wunan';

IF @Keywords = N'' THROW 54001,'Keywords is empty.',1;

SELECT Subject.Id, Subject.Name, Person.LastName, Person.FirstName, Person.PhoneticDisplayName,
    SubjectIdentity.IdentifierType, SubjectIdentity.IdentifierValue, Person.Mobile, Person.Sex, Subject.WhenCreated,
    Subject.WhenChanged, Subject.Enabled, ChineseIDCardIdentity.Address, ChineseIDCardIdentity.Issuer,
    ChineseIDCardIdentity.IssueDate, ChineseIDCardIdentity.Expires
FROM ChineseIDCardIdentity INNER JOIN
    SubjectIdentity ON ChineseIDCardIdentity.IdentifierType = SubjectIdentity.IdentifierType AND
        ChineseIDCardIdentity.IdentifierValue = SubjectIdentity.IdentifierValue RIGHT OUTER JOIN
    Subject INNER JOIN
    Person ON Subject.Id = Person.Id ON SubjectIdentity.SubjectId = Subject.Id
WHERE   (Subject.Name LIKE N'' + @Keywords + N'%') OR
    (Person.PhoneticDisplayName LIKE N'' + @Keywords + N'%') OR
    (Person.Mobile LIKE N'' + @Keywords + N'') OR
    (SubjectIdentity.IdentifierValue = @Keywords)
ORDER BY Subject.Name