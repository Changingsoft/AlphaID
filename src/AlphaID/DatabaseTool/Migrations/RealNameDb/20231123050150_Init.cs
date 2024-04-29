#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.RealNameDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "IdentityDocument",
            table => new
            {
                Id = table.Column<string>("varchar(50)", false, 50, nullable: false),
                WhenCreated = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                Discriminator = table.Column<string>("varchar(100)", false, 100, nullable: false),
                Name = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Sex = table.Column<string>("varchar(7)", nullable: true),
                Ethnicity = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                DateOfBirth = table.Column<DateTime>("date", nullable: true),
                Address = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                CardNumber = table.Column<string>("varchar(18)", false, 18, nullable: true),
                Issuer = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                IssueDate = table.Column<DateTime>("date", nullable: true),
                Expires = table.Column<DateTime>("date", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_IdentityDocument", x => x.Id); });

        migrationBuilder.CreateTable(
            "RealNameRequest",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PersonId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                WhenCommitted = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                Accepted = table.Column<bool>("bit", nullable: true),
                Auditor = table.Column<string>("nvarchar(max)", nullable: true),
                AcceptedAt = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                Discriminator = table.Column<string>("varchar(100)", false, 100, nullable: false),
                PersonalSide_MimeType = table.Column<string>("varchar(100)", false, 100, nullable: true),
                PersonalSide_Data = table.Column<byte[]>("varbinary(max)", nullable: true),
                PersonalSide_UpdateTime = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                IssuerSide_MimeType = table.Column<string>("varchar(100)", false, 100, nullable: true),
                IssuerSide_Data = table.Column<byte[]>("varbinary(max)", nullable: true),
                IssuerSide_UpdateTime = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                Expires = table.Column<DateTime>("date", nullable: true),
                IssueDate = table.Column<DateTime>("date", nullable: true),
                Issuer = table.Column<string>("nvarchar(max)", nullable: true),
                CardNumber = table.Column<string>("nvarchar(max)", nullable: true),
                Ethnicity = table.Column<string>("nvarchar(max)", nullable: true),
                DateOfBirth = table.Column<DateTime>("date", nullable: true),
                Sex = table.Column<int>("int", nullable: true),
                Name = table.Column<string>("nvarchar(max)", nullable: true),
                Address = table.Column<string>("nvarchar(max)", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_RealNameRequest", x => x.Id); });

        migrationBuilder.CreateTable(
            "IdentityDocumentAttachment",
            table => new
            {
                DocumentId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                Name = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: false),
                Content = table.Column<byte[]>("varbinary(max)", nullable: false),
                ContentType = table.Column<string>("varchar(100)", false, 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IdentityDocumentAttachment", x => new { x.DocumentId, x.Name });
                table.ForeignKey(
                    "FK_IdentityDocumentAttachment_IdentityDocument_DocumentId",
                    x => x.DocumentId,
                    "IdentityDocument",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "RealNameAuthentication",
            table => new
            {
                Id = table.Column<string>("varchar(50)", false, 50, nullable: false),
                PersonId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                PersonName_Surname = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                PersonName_MiddleName = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                PersonName_GivenName = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                PersonName_FullName = table.Column<string>("nvarchar(150)", maxLength: 150, nullable: false),
                PersonName_SearchHint = table.Column<string>("nvarchar(60)", maxLength: 60, nullable: true),
                ValidatedAt = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                ValidatedBy = table.Column<string>("nvarchar(max)", nullable: false),
                ExpiresAt = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                Remark = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                Applied = table.Column<bool>("bit", nullable: false),
                Discriminator = table.Column<string>("varchar(100)", false, 100, nullable: false),
                DocumentId = table.Column<string>("varchar(50)", false, 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RealNameAuthentication", x => x.Id);
                table.ForeignKey(
                    "FK_RealNameAuthentication_IdentityDocument_DocumentId",
                    x => x.DocumentId,
                    "IdentityDocument",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_RealNameAuthentication_DocumentId",
            "RealNameAuthentication",
            "DocumentId");

        migrationBuilder.CreateIndex(
            "IX_RealNameAuthentication_PersonName_FullName",
            "RealNameAuthentication",
            "PersonName_FullName");

        migrationBuilder.CreateIndex(
            "IX_RealNameAuthentication_PersonName_SearchHint",
            "RealNameAuthentication",
            "PersonName_SearchHint");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "IdentityDocumentAttachment");

        migrationBuilder.DropTable(
            "RealNameAuthentication");

        migrationBuilder.DropTable(
            "RealNameRequest");

        migrationBuilder.DropTable(
            "IdentityDocument");
    }
}