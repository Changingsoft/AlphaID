using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.RealNameDb
{
    /// <inheritdoc />
    public partial class AddRealNameActionIndicator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionIndicator",
                table: "RealNameState",
                type: "varchar(15)",
                nullable: false,
                defaultValue: "Updated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionIndicator",
                table: "RealNameState");
        }
    }
}
