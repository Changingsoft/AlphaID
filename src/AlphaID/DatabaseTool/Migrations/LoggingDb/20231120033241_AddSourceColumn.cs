#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.LoggingDb;

/// <inheritdoc />
public partial class AddSourceColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            "Source",
            "AuditLog",
            "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "Source",
            "AuditLog");
    }
}