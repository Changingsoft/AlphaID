#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.PersistedGrantDb;

/// <inheritdoc />
public partial class Update_IdentityServer_v7_0 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey("PK_ServerSideSessions", "ServerSideSessions");

        migrationBuilder.AlterColumn<long>(
                "Id",
                "ServerSideSessions",
                "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
            .Annotation("SqlServer:Identity", "1, 1")
            .OldAnnotation("SqlServer:Identity", "1, 1");

        migrationBuilder.AddPrimaryKey("PK_ServerSideSessions", "ServerSideSessions", "Id");

        migrationBuilder.CreateTable(
            "PushedAuthorizationRequests",
            table => new
            {
                Id = table.Column<long>("bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ReferenceValueHash = table.Column<string>("nvarchar(64)", maxLength: 64, nullable: false),
                ExpiresAtUtc = table.Column<DateTime>("datetime2", nullable: false),
                Parameters = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_PushedAuthorizationRequests", x => x.Id); });

        migrationBuilder.CreateIndex(
            "IX_PushedAuthorizationRequests_ExpiresAtUtc",
            "PushedAuthorizationRequests",
            "ExpiresAtUtc");

        migrationBuilder.CreateIndex(
            "IX_PushedAuthorizationRequests_ReferenceValueHash",
            "PushedAuthorizationRequests",
            "ReferenceValueHash",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "PushedAuthorizationRequests");

        migrationBuilder.AlterColumn<int>(
                "Id",
                "ServerSideSessions",
                "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
            .Annotation("SqlServer:Identity", "1, 1")
            .OldAnnotation("SqlServer:Identity", "1, 1");
    }
}