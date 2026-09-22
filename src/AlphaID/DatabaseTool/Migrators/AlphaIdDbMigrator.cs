using AlphaId.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;

internal class AlphaIdDbMigrator(AlphaIdDbContext db) : DatabaseMigrator(db);