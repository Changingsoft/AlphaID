#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.ConfigurationDb;

/// <inheritdoc />
public partial class Update_IdentityServer_v7_0 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            "PushedAuthorizationLifetime",
            "Clients",
            "int",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            "RequirePushedAuthorization",
            "Clients",
            "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "PushedAuthorizationLifetime",
            "Clients");

        migrationBuilder.DropColumn(
            "RequirePushedAuthorization",
            "Clients");
    }
}