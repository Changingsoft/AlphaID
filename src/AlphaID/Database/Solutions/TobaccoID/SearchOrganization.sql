USE TobaccoIDData
GO

--要检索的名称。
DECLARE @Keywords nvarchar(20) = N'';

IF @Keywords = ''
THROW 54001, 'Keywords is empty.', 1;

SELECT Subject.Id, Subject.Name, Subject.Domicile, Subject.Contact, Subject.WhenCreated, Subject.WhenChanged,
    Organization.LegalPersonName, Organization.WhenRegistered, Organization.Expires, Subject.Enabled
FROM Subject INNER JOIN
    Organization ON Subject.Id = Organization.Id
WHERE   (Subject.Name LIKE N'%' + @Keywords + '%')

SELECT Subject.Id, Subject.Name, SubjectIdentity.IdentifierType, SubjectIdentity.IdentifierValue
FROM Subject INNER JOIN
    Organization ON Subject.Id = Organization.Id INNER JOIN
    SubjectIdentity ON Subject.Id = SubjectIdentity.SubjectId
WHERE   (Subject.Name LIKE N'%' + @Keywords + N'%')