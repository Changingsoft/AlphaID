#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.PersistedGrantDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "DeviceCodes",
            table => new
            {
                UserCode = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: false),
                DeviceCode = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: false),
                SubjectId = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                SessionId = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                ClientId = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                CreationTime = table.Column<DateTime>("datetime2", nullable: false),
                Expiration = table.Column<DateTime>("datetime2", nullable: false),
                Data = table.Column<string>("nvarchar(max)", maxLength: 50000, nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_DeviceCodes", x => x.UserCode); });

        migrationBuilder.CreateTable(
            "Keys",
            table => new
            {
                Id = table.Column<string>("nvarchar(450)", nullable: false),
                Version = table.Column<int>("int", nullable: false),
                Created = table.Column<DateTime>("datetime2", nullable: false),
                Use = table.Column<string>("nvarchar(450)", nullable: true),
                Algorithm = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: false),
                IsX509Certificate = table.Column<bool>("bit", nullable: false),
                DataProtected = table.Column<bool>("bit", nullable: false),
                Data = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Keys", x => x.Id); });

        migrationBuilder.CreateTable(
            "PersistedGrants",
            table => new
            {
                Id = table.Column<long>("bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Key = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                Type = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: false),
                SubjectId = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                SessionId = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                ClientId = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                CreationTime = table.Column<DateTime>("datetime2", nullable: false),
                Expiration = table.Column<DateTime>("datetime2", nullable: true),
                ConsumedTime = table.Column<DateTime>("datetime2", nullable: true),
                Data = table.Column<string>("nvarchar(max)", maxLength: 50000, nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_PersistedGrants", x => x.Id); });

        migrationBuilder.CreateTable(
            "ServerSideSessions",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Key = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: false),
                Scheme = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: false),
                SubjectId = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: false),
                SessionId = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                DisplayName = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                Created = table.Column<DateTime>("datetime2", nullable: false),
                Renewed = table.Column<DateTime>("datetime2", nullable: false),
                Expires = table.Column<DateTime>("datetime2", nullable: true),
                Data = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_ServerSideSessions", x => x.Id); });

        migrationBuilder.CreateIndex(
            "IX_DeviceCodes_DeviceCode",
            "DeviceCodes",
            "DeviceCode",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_DeviceCodes_Expiration",
            "DeviceCodes",
            "Expiration");

        migrationBuilder.CreateIndex(
            "IX_Keys_Use",
            "Keys",
            "Use");

        migrationBuilder.CreateIndex(
            "IX_PersistedGrants_ConsumedTime",
            "PersistedGrants",
            "ConsumedTime");

        migrationBuilder.CreateIndex(
            "IX_PersistedGrants_Expiration",
            "PersistedGrants",
            "Expiration");

        migrationBuilder.CreateIndex(
            "IX_PersistedGrants_Key",
            "PersistedGrants",
            "Key",
            unique: true,
            filter: "[Key] IS NOT NULL");

        migrationBuilder.CreateIndex(
            "IX_PersistedGrants_SubjectId_ClientId_Type",
            "PersistedGrants",
            ["SubjectId", "ClientId", "Type"]);

        migrationBuilder.CreateIndex(
            "IX_PersistedGrants_SubjectId_SessionId_Type",
            "PersistedGrants",
            ["SubjectId", "SessionId", "Type"]);

        migrationBuilder.CreateIndex(
            "IX_ServerSideSessions_DisplayName",
            "ServerSideSessions",
            "DisplayName");

        migrationBuilder.CreateIndex(
            "IX_ServerSideSessions_Expires",
            "ServerSideSessions",
            "Expires");

        migrationBuilder.CreateIndex(
            "IX_ServerSideSessions_Key",
            "ServerSideSessions",
            "Key",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_ServerSideSessions_SessionId",
            "ServerSideSessions",
            "SessionId");

        migrationBuilder.CreateIndex(
            "IX_ServerSideSessions_SubjectId",
            "ServerSideSessions",
            "SubjectId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "DeviceCodes");

        migrationBuilder.DropTable(
            "Keys");

        migrationBuilder.DropTable(
            "PersistedGrants");

        migrationBuilder.DropTable(
            "ServerSideSessions");
    }
}