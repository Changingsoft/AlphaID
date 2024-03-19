using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.RealNameDb
{
    /// <inheritdoc />
    public partial class PatchColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AcceptedAt",
                table: "RealNameRequest",
                newName: "AuditTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuditTime",
                table: "RealNameRequest",
                newName: "AcceptedAt");
        }
    }
}
