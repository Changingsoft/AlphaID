using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.IdSubjectsDb
{
    /// <inheritdoc />
    public partial class RenameReceiver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationIdentifier_Organization_OrganizationId",
                table: "OrganizationIdentifier");

            migrationBuilder.DropIndex(
                name: "IX_Organization_Name",
                table: "Organization");

            migrationBuilder.RenameColumn(
                name: "Address_Receiver",
                table: "ApplicationUser",
                newName: "Address_Recipient");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationIdentifier",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationIdentifier_Organization_OrganizationId",
                table: "OrganizationIdentifier",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationIdentifier_Organization_OrganizationId",
                table: "OrganizationIdentifier");

            migrationBuilder.DropIndex(
                name: "IX_Organization_Name",
                table: "Organization");

            migrationBuilder.RenameColumn(
                name: "Address_Recipient",
                table: "ApplicationUser",
                newName: "Address_Receiver");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationIdentifier",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationIdentifier_Organization_OrganizationId",
                table: "OrganizationIdentifier",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
