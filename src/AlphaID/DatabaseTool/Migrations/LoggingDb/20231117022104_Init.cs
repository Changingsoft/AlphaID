#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.LoggingDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "AuditLog",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Message = table.Column<string>("nvarchar(max)", nullable: true),
                MessageTemplate = table.Column<string>("nvarchar(max)", nullable: true),
                Level = table.Column<string>("nvarchar(128)", maxLength: 128, nullable: true),
                TimeStamp = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                Exception = table.Column<string>("nvarchar(max)", nullable: true),
                Properties = table.Column<string>("nvarchar(max)", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_AuditLog", x => x.Id); });

        migrationBuilder.CreateIndex(
            "IX_AuditLog_TimeStamp",
            "AuditLog",
            "TimeStamp");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "AuditLog");
    }
}