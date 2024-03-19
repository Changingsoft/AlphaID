using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.RealNameDb
{
    /// <inheritdoc />
    public partial class RenameRemarkColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "RealNameAuthentication",
                newName: "Remarks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "RealNameAuthentication",
                newName: "Remark");
        }
    }
}
