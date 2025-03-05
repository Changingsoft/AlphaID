using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.IdSubjectsDb
{
    /// <inheritdoc />
    public partial class PersonIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationIdentifier_Organization_OrganizationId",
                table: "OrganizationIdentifier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationBankAccount",
                table: "OrganizationBankAccount");

            migrationBuilder.DropIndex(
                name: "IX_Organization_Name",
                table: "Organization");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationIdentifier",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "ApplicationUser",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationBankAccount",
                table: "OrganizationBankAccount",
                columns: new[] { "AccountNumber", "OrganizationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Name",
                table: "ApplicationUser",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_SearchHint",
                table: "ApplicationUser",
                column: "SearchHint");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationBankAccount",
                table: "OrganizationBankAccount");

            migrationBuilder.DropIndex(
                name: "IX_Organization_Name",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_Name",
                table: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_SearchHint",
                table: "ApplicationUser");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationIdentifier",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "ApplicationUser",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationBankAccount",
                table: "OrganizationBankAccount",
                column: "AccountNumber");

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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
