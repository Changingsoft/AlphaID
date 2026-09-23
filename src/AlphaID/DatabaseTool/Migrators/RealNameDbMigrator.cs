using AlphaId.EntityFramework.RealName;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;

internal class RealNameDbMigrator(RealNameDbContext db) : DatabaseMigrator(db);