using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.DirectoryLogonDb
{
    /// <inheritdoc />
    public partial class RemoveExtLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalLoginProvider_DisplayName",
                table: "DirectoryService");

            migrationBuilder.DropColumn(
                name: "ExternalLoginProvider_Name",
                table: "DirectoryService");

            migrationBuilder.DropColumn(
                name: "ExternalLoginProvider_RegisteredClientId",
                table: "DirectoryService");

            migrationBuilder.DropColumn(
                name: "ExternalLoginProvider_SubjectGenerator",
                table: "DirectoryService");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProvider_DisplayName",
                table: "DirectoryService",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProvider_Name",
                table: "DirectoryService",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProvider_RegisteredClientId",
                table: "DirectoryService",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProvider_SubjectGenerator",
                table: "DirectoryService",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true);
        }
    }
}
