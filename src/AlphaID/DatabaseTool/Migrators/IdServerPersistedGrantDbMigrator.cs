using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;

internal class IdServerPersistedGrantDbMigrator(PersistedGrantDbContext db) : DatabaseMigrator(db);