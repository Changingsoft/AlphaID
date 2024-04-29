#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.RealNameDb;

/// <inheritdoc />
public partial class RenameRemarkColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "Remark",
            "RealNameAuthentication",
            "Remarks");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "Remarks",
            "RealNameAuthentication",
            "Remark");
    }
}