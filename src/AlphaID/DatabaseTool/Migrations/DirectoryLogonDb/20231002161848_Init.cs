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
                    ServerAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RootDN = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DefaultUserAccountOU = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UpnSuffix = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SAMDomainPart = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    ExternalLoginProvider = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ExternalLoginProviderName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryService", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogonAccount",
                columns: table => new
                {
                    LogonId = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false, collation: "Chinese_PRC_CS_AS"),
                    PersonId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogonAccount", x => x.LogonId);
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
