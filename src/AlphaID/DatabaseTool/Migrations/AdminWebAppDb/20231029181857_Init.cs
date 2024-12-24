#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.AdminWebAppDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "UserInRole",
            table => new
            {
                UserId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                RoleName = table.Column<string>("varchar(50)", false, 50, nullable: false),
                UserName = table.Column<string>("nvarchar(20)", maxLength: 20, nullable: false),
                UserSearchHint = table.Column<string>("varchar(50)", false, 50, nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_UserInRole", x => new { x.UserId, x.RoleName }); });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "UserInRole");
    }
}