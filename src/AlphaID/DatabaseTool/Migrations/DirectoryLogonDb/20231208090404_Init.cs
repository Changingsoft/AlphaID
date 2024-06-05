#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.DirectoryLogonDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "DirectoryService",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: false),
                Type = table.Column<string>("varchar(10)", nullable: false),
                ServerAddress = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: false),
                RootDn = table.Column<string>("nvarchar(150)", maxLength: 150, nullable: false),
                DefaultUserAccountContainer = table.Column<string>("nvarchar(150)", maxLength: 150, nullable: false),
                UserName = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Password = table.Column<string>("varchar(50)", false, 50, nullable: true),
                UpnSuffix = table.Column<string>("varchar(20)", false, 20, nullable: false),
                SamDomainPart = table.Column<string>("varchar(10)", false, 10, nullable: true),
                ExternalLoginProvider_Name = table.Column<string>("varchar(50)", false, 50, nullable: true),
                ExternalLoginProvider_DisplayName = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                ExternalLoginProvider_RegisteredClientId =
                    table.Column<string>("varchar(50)", false, 50, nullable: true),
                ExternalLoginProvider_SubjectGenerator = table.Column<string>("varchar(50)", false, 50, nullable: true),
                AutoCreateAccount = table.Column<bool>("bit", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_DirectoryService", x => x.Id); });

        migrationBuilder.CreateTable(
            "LogonAccount",
            table => new
            {
                PersonId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                ServiceId = table.Column<int>("int", nullable: false),
                ObjectId = table.Column<string>("varchar(50)", false, 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LogonAccount", x => new { x.PersonId, x.ServiceId });
                table.ForeignKey(
                    "FK_LogonAccount_DirectoryService_ServiceId",
                    x => x.ServiceId,
                    "DirectoryService",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_LogonAccount_ServiceId",
            "LogonAccount",
            "ServiceId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "LogonAccount");

        migrationBuilder.DropTable(
            "DirectoryService");
    }
}