using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.DirectoryLogonDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectoryService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "varchar(10)", nullable: false),
                    ServerAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RootDn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DefaultUserAccountContainer = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UpnSuffix = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SamDomainPart = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    ExternalLoginProvider_Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ExternalLoginProvider_DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExternalLoginProvider_RegisteredClientId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ExternalLoginProvider_SubjectGenerator = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    AutoCreateAccount = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryService", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogonAccount",
                columns: table => new
                {
                    PersonId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    ObjectId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogonAccount", x => new { x.PersonId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_LogonAccount_DirectoryService_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "DirectoryService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogonAccount_ServiceId",
                table: "LogonAccount",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogonAccount");

            migrationBuilder.DropTable(
                name: "DirectoryService");
        }
    }
}
