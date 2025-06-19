using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.IdSubjectsDb
{
    /// <inheritdoc />
    public partial class OrgMemberOwned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMember_ApplicationUser_PersonId",
                table: "OrganizationMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationMember",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_OrganizationId",
                table: "OrganizationMember");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationMember",
                table: "OrganizationMember",
                columns: new[] { "OrganizationId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_PersonId",
                table: "OrganizationMember",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationMember",
                table: "OrganizationMember");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMember_PersonId",
                table: "OrganizationMember");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationMember",
                table: "OrganizationMember",
                columns: new[] { "PersonId", "OrganizationId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_OrganizationId",
                table: "OrganizationMember",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMember_ApplicationUser_PersonId",
                table: "OrganizationMember",
                column: "PersonId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
