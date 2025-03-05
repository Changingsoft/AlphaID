using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.IdSubjectsDb
{
    /// <inheritdoc />
    public partial class PersonBankAccountPKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationIdentifier_Organization_OrganizationId",
                table: "OrganizationIdentifier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NaturalPersonBankAccount",
                table: "NaturalPersonBankAccount");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationIdentifier",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NaturalPersonBankAccount",
                table: "NaturalPersonBankAccount",
                columns: new[] { "AccountNumber", "NaturalPersonId" });

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
                name: "PK_NaturalPersonBankAccount",
                table: "NaturalPersonBankAccount");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "OrganizationIdentifier",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NaturalPersonBankAccount",
                table: "NaturalPersonBankAccount",
                column: "AccountNumber");

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
