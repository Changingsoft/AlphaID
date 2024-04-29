#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.RealNameDb;

/// <inheritdoc />
public partial class PatchColumnName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "AcceptedAt",
            "RealNameRequest",
            "AuditTime");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "AuditTime",
            "RealNameRequest",
            "AcceptedAt");
    }
}