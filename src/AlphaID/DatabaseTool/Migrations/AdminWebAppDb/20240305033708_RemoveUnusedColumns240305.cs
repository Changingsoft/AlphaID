#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.AdminWebAppDb;

/// <inheritdoc />
public partial class RemoveUnusedColumns240305 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "UserName",
            "UserInRole");

        migrationBuilder.DropColumn(
            "UserSearchHint",
            "UserInRole");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            "UserName",
            "UserInRole",
            "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "UserSearchHint",
            "UserInRole",
            "varchar(50)",
            false,
            50,
            nullable: false,
            defaultValue: "");
    }
}