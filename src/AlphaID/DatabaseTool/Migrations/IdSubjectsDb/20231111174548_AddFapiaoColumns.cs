using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.IdSubjectsDb
{
    /// <inheritdoc />
    public partial class AddFapiaoColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fapiao_Account",
                table: "Organization",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fapiao_Address",
                table: "Organization",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fapiao_Bank",
                table: "Organization",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fapiao_Contact",
                table: "Organization",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fapiao_Name",
                table: "Organization",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fapiao_TaxPayerId",
                table: "Organization",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fapiao_Account",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Fapiao_Address",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Fapiao_Bank",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Fapiao_Contact",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Fapiao_Name",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Fapiao_TaxPayerId",
                table: "Organization");
        }
    }
}
