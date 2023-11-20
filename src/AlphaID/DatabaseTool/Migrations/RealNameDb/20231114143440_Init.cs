using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.RealNameDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityDocument",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    WhenCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sex = table.Column<string>(type: "varchar(7)", nullable: true),
                    Ethnicity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CardNumber = table.Column<string>(type: "varchar(18)", unicode: false, maxLength: 18, nullable: true),
                    Issuer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: true),
                    Expires = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDocument", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RealNameState",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AcceptedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealNameState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAttachment",
                columns: table => new
                {
                    DocumentId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAttachment", x => new { x.DocumentId, x.Name });
                    table.ForeignKey(
                        name: "FK_DocumentAttachment_IdentityDocument_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "IdentityDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealNameValidation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RealNameStateId = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealNameValidation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RealNameValidation_IdentityDocument_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "IdentityDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealNameValidation_RealNameState_RealNameStateId",
                        column: x => x.RealNameStateId,
                        principalTable: "RealNameState",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RealNameValidation_DocumentId",
                table: "RealNameValidation",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RealNameValidation_RealNameStateId",
                table: "RealNameValidation",
                column: "RealNameStateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAttachment");

            migrationBuilder.DropTable(
                name: "RealNameValidation");

            migrationBuilder.DropTable(
                name: "IdentityDocument");

            migrationBuilder.DropTable(
                name: "RealNameState");
        }
    }
}
