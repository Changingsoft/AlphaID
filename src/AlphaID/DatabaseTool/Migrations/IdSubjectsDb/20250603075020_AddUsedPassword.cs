using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.IdSubjectsDb
{
    /// <inheritdoc />
    public partial class AddUsedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordHistory");

            migrationBuilder.CreateTable(
                name: "UsedPassword",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NaturalPersonId = table.Column<string>(type: "varchar(50)", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedPassword", x => new { x.NaturalPersonId, x.Id });
                    table.ForeignKey(
                        name: "FK_UsedPassword_ApplicationUser_NaturalPersonId",
                        column: x => x.NaturalPersonId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedPassword");

            migrationBuilder.CreateTable(
                name: "PasswordHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    WhenCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistory_WhenCreated",
                table: "PasswordHistory",
                column: "WhenCreated");
        }
    }
}
