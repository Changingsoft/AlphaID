using AlphaId.EntityFramework.DirectoryAccountManagement;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AlphaId.EntityFramework.IdSubjects;
using IdSubjects.DirectoryLogon;

namespace DatabaseTool.Migrators;

internal class DirectoryLogonDbMigrator(DirectoryLogonDbContext db) : DatabaseMigrator(db);