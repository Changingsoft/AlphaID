using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace DatabaseTool.Migrations.AlphaIdDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JoinOrganizationInvitation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InviteeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    WhenCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WhenExpired = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Inviter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpectVisibility = table.Column<int>(type: "int", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinOrganizationInvitation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JoinOrganizationRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    WhenCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AuditAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AuditBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinOrganizationRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Domicile = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Representative = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    USCC = table.Column<string>(type: "varchar(18)", unicode: false, maxLength: 18, nullable: true),
                    DUNS = table.Column<string>(type: "varchar(9)", unicode: false, maxLength: 9, nullable: true),
                    LEI = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ProfilePicture_MimeType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ProfilePicture_Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    WhenCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WhenChanged = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    EstablishedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    TermBegin = table.Column<DateOnly>(type: "date", nullable: true),
                    TermEnd = table.Column<DateOnly>(type: "date", nullable: true),
                    Location = table.Column<Geometry>(type: "geography", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Fapiao_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fapiao_TaxPayerId = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Fapiao_Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Fapiao_Contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fapiao_Bank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fapiao_Account = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationBankAccount",
                columns: table => new
                {
                    AccountNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    OrganizationId = table.Column<string>(type: "varchar(50)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Usage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Default = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationBankAccount", x => new { x.OrganizationId, x.AccountNumber });
                    table.ForeignKey(
                        name: "FK_OrganizationBankAccount_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMember",
                columns: table => new
                {
                    PersonId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    OrganizationId = table.Column<string>(type: "varchar(50)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMember", x => new { x.OrganizationId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_OrganizationMember_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUsedName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeprecateTime = table.Column<DateOnly>(type: "date", nullable: false),
                    OrganizationId = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUsedName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationUsedName_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinOrganizationRequest_WhenCreated",
                table: "JoinOrganizationRequest",
                column: "WhenCreated");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Name",
                table: "Organization",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_USCC",
                table: "Organization",
                column: "USCC",
                unique: true,
                filter: "[USCC] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_WhenChanged",
                table: "Organization",
                column: "WhenChanged");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_WhenCreated",
                table: "Organization",
                column: "WhenCreated");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_PersonId",
                table: "OrganizationMember",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsedName_OrganizationId",
                table: "OrganizationUsedName",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoinOrganizationInvitation");

            migrationBuilder.DropTable(
                name: "JoinOrganizationRequest");

            migrationBuilder.DropTable(
                name: "OrganizationBankAccount");

            migrationBuilder.DropTable(
                name: "OrganizationMember");

            migrationBuilder.DropTable(
                name: "OrganizationUsedName");

            migrationBuilder.DropTable(
                name: "Organization");
        }
    }
}
