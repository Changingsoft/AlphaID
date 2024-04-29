#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace DatabaseTool.Migrations.IdSubjectsDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "NaturalPerson",
            table => new
            {
                Id = table.Column<string>("varchar(50)", false, 50, nullable: false),
                UserName = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: false),
                NormalizedUserName = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: false),
                Email = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>("bit", nullable: false),
                PhoneNumber = table.Column<string>("varchar(20)", false, 20, nullable: true),
                PhoneNumberConfirmed = table.Column<bool>("bit", nullable: false),
                PasswordHash = table.Column<string>("varchar(100)", false, 100, nullable: true),
                PasswordLastSet = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                SecurityStamp = table.Column<string>("varchar(50)", false, 50, nullable: true),
                ConcurrencyStamp = table.Column<string>("varchar(50)", false, 50, nullable: true),
                TwoFactorEnabled = table.Column<bool>("bit", nullable: false),
                LockoutEnabled = table.Column<bool>("bit", nullable: false),
                AccessFailedCount = table.Column<int>("int", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                WhenCreated = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                WhenChanged = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                PersonWhenChanged = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                Enabled = table.Column<bool>("bit", nullable: false),
                PersonName_Surname = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                PersonName_MiddleName = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                PersonName_GivenName = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                PersonName_FullName = table.Column<string>("nvarchar(150)", maxLength: 150, nullable: false),
                PersonName_SearchHint = table.Column<string>("nvarchar(60)", maxLength: 60, nullable: true),
                NickName = table.Column<string>("nvarchar(20)", maxLength: 20, nullable: true),
                Gender = table.Column<string>("varchar(6)", nullable: true, comment: "性别"),
                DateOfBirth = table.Column<DateTime>("date", nullable: true),
                Bio = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                PhoneticSurname = table.Column<string>("varchar(20)", false, 20, nullable: true),
                PhoneticGivenName = table.Column<string>("varchar(40)", false, 40, nullable: true),
                ProfilePicture_MimeType = table.Column<string>("varchar(100)", false, 100, nullable: true),
                ProfilePicture_Data = table.Column<byte[]>("varbinary(max)", nullable: true),
                ProfilePicture_UpdateTime = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                Locale = table.Column<string>("varchar(10)", false, 10, nullable: true),
                TimeZone = table.Column<string>("varchar(50)", false, 50, nullable: true),
                Address_Country = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Address_Region = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Address_Locality = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Address_Street1 = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Address_Street2 = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Address_Street3 = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Address_Receiver = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Address_Contact = table.Column<string>("varchar(20)", false, 20, nullable: true),
                Address_PostalCode = table.Column<string>("varchar(20)", false, 20, nullable: true),
                WebSite = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_NaturalPerson", x => x.Id); });

        migrationBuilder.CreateTable(
            "Organization",
            table => new
            {
                Id = table.Column<string>("varchar(50)", false, 50, nullable: false),
                Name = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: false),
                Domicile = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                Contact = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Email = table.Column<string>("varchar(100)", false, 100, nullable: true),
                Representative = table.Column<string>("nvarchar(20)", maxLength: 20, nullable: true),
                ProfilePicture_MimeType = table.Column<string>("varchar(100)", false, 100, nullable: true),
                ProfilePicture_Data = table.Column<byte[]>("varbinary(max)", nullable: true),
                ProfilePicture_UpdateTime = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                WhenCreated = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                WhenChanged = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                Enabled = table.Column<bool>("bit", nullable: false),
                EstablishedAt = table.Column<DateTime>("date", nullable: true),
                TermBegin = table.Column<DateTime>("date", nullable: true),
                TermEnd = table.Column<DateTime>("date", nullable: true),
                Location = table.Column<Geometry>("geography", nullable: true),
                Website = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true),
                Description = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: true),
                Fapiao_Name = table.Column<string>("nvarchar(30)", maxLength: 30, nullable: true),
                Fapiao_TaxPayerId = table.Column<string>("nvarchar(30)", maxLength: 30, nullable: true),
                Fapiao_Address = table.Column<string>("nvarchar(30)", maxLength: 30, nullable: true),
                Fapiao_Contact = table.Column<string>("nvarchar(30)", maxLength: 30, nullable: true),
                Fapiao_Bank = table.Column<string>("nvarchar(30)", maxLength: 30, nullable: true),
                Fapiao_Account = table.Column<string>("nvarchar(30)", maxLength: 30, nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_Organization", x => x.Id); });

        migrationBuilder.CreateTable(
            "NaturalPersonClaim",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                ClaimType = table.Column<string>("varchar(200)", false, 200, nullable: false),
                ClaimValue = table.Column<string>("nvarchar(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NaturalPersonClaim", x => x.Id);
                table.ForeignKey(
                    "FK_NaturalPersonClaim_NaturalPerson_UserId",
                    x => x.UserId,
                    "NaturalPerson",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "NaturalPersonLogin",
            table => new
            {
                LoginProvider = table.Column<string>("varchar(50)", false, 50, nullable: false),
                ProviderKey = table.Column<string>("varchar(128)", false, 128, nullable: false),
                ProviderDisplayName = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                UserId = table.Column<string>("varchar(50)", false, 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NaturalPersonLogin", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    "FK_NaturalPersonLogin_NaturalPerson_UserId",
                    x => x.UserId,
                    "NaturalPerson",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "NaturalPersonToken",
            table => new
            {
                UserId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                LoginProvider = table.Column<string>("varchar(50)", false, 50, nullable: false),
                Name = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: false),
                Value = table.Column<string>("nvarchar(256)", maxLength: 256, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NaturalPersonToken", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    "FK_NaturalPersonToken_NaturalPerson_UserId",
                    x => x.UserId,
                    "NaturalPerson",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "PasswordHistory",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                Data = table.Column<string>("varchar(100)", false, 100, nullable: false),
                WhenCreated = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PasswordHistory", x => x.Id);
                table.ForeignKey(
                    "FK_PasswordHistory_NaturalPerson_UserId",
                    x => x.UserId,
                    "NaturalPerson",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "PersonBankAccount",
            table => new
            {
                AccountNumber = table.Column<string>("varchar(50)", false, 50, nullable: false),
                PersonId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                AccountName = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                BankName = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PersonBankAccount", x => new { x.AccountNumber, x.PersonId });
                table.ForeignKey(
                    "FK_PersonBankAccount_NaturalPerson_PersonId",
                    x => x.PersonId,
                    "NaturalPerson",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "JoinOrganizationInvitation",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                InviteeId = table.Column<string>("varchar(50)", maxLength: 50, nullable: false),
                OrganizationId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                WhenCreated = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                WhenExpired = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                Inviter = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: false),
                ExpectVisibility = table.Column<int>("int", nullable: false),
                Accepted = table.Column<bool>("bit", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JoinOrganizationInvitation", x => x.Id);
                table.ForeignKey(
                    "FK_JoinOrganizationInvitation_NaturalPerson_InviteeId",
                    x => x.InviteeId,
                    "NaturalPerson",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_JoinOrganizationInvitation_Organization_OrganizationId",
                    x => x.OrganizationId,
                    "Organization",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "OrganizationBankAccount",
            table => new
            {
                AccountNumber = table.Column<string>("varchar(50)", false, 50, nullable: false),
                OrganizationId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                AccountName = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: false),
                BankName = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: true),
                Usage = table.Column<string>("nvarchar(20)", maxLength: 20, nullable: true),
                Default = table.Column<bool>("bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrganizationBankAccount", x => new { x.AccountNumber, x.OrganizationId });
                table.ForeignKey(
                    "FK_OrganizationBankAccount_Organization_OrganizationId",
                    x => x.OrganizationId,
                    "Organization",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "OrganizationIdentifier",
            table => new
            {
                Type = table.Column<string>("varchar(30)", nullable: false),
                Value = table.Column<string>("nvarchar(30)", maxLength: 30, nullable: false),
                OrganizationId = table.Column<string>("varchar(50)", false, 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrganizationIdentifier", x => new { x.Value, x.Type });
                table.ForeignKey(
                    "FK_OrganizationIdentifier_Organization_OrganizationId",
                    x => x.OrganizationId,
                    "Organization",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "OrganizationMember",
            table => new
            {
                OrganizationId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                PersonId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                Department = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Title = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                Remark = table.Column<string>("nvarchar(50)", maxLength: 50, nullable: true),
                IsOwner = table.Column<bool>("bit", nullable: false),
                Visibility = table.Column<int>("int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrganizationMember", x => new { x.PersonId, x.OrganizationId });
                table.ForeignKey(
                    "FK_OrganizationMember_NaturalPerson_PersonId",
                    x => x.PersonId,
                    "NaturalPerson",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_OrganizationMember_Organization_OrganizationId",
                    x => x.OrganizationId,
                    "Organization",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "OrganizationUsedName",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OrganizationId = table.Column<string>("varchar(50)", false, 50, nullable: false),
                Name = table.Column<string>("nvarchar(100)", maxLength: 100, nullable: false),
                DeprecateTime = table.Column<DateTime>("date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrganizationUsedName", x => x.Id);
                table.ForeignKey(
                    "FK_OrganizationUsedName_Organization_OrganizationId",
                    x => x.OrganizationId,
                    "Organization",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_JoinOrganizationInvitation_InviteeId",
            "JoinOrganizationInvitation",
            "InviteeId");

        migrationBuilder.CreateIndex(
            "IX_JoinOrganizationInvitation_OrganizationId",
            "JoinOrganizationInvitation",
            "OrganizationId");

        migrationBuilder.CreateIndex(
            "IX_NaturalPerson_NormalizedUserName",
            "NaturalPerson",
            "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_NaturalPerson_PersonName_FullName",
            "NaturalPerson",
            "PersonName_FullName");

        migrationBuilder.CreateIndex(
            "IX_NaturalPerson_PersonName_SearchHint",
            "NaturalPerson",
            "PersonName_SearchHint");

        migrationBuilder.CreateIndex(
            "IX_NaturalPerson_UserName",
            "NaturalPerson",
            "UserName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_NaturalPerson_WhenChanged",
            "NaturalPerson",
            "WhenChanged");

        migrationBuilder.CreateIndex(
            "IX_NaturalPerson_WhenCreated",
            "NaturalPerson",
            "WhenCreated");

        migrationBuilder.CreateIndex(
            "IX_NaturalPersonClaim_UserId",
            "NaturalPersonClaim",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_NaturalPersonLogin_UserId",
            "NaturalPersonLogin",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Organization_Name",
            "Organization",
            "Name");

        migrationBuilder.CreateIndex(
            "IX_Organization_WhenChanged",
            "Organization",
            "WhenChanged");

        migrationBuilder.CreateIndex(
            "IX_Organization_WhenCreated",
            "Organization",
            "WhenCreated");

        migrationBuilder.CreateIndex(
            "IX_OrganizationBankAccount_OrganizationId",
            "OrganizationBankAccount",
            "OrganizationId");

        migrationBuilder.CreateIndex(
            "IX_OrganizationIdentifier_OrganizationId",
            "OrganizationIdentifier",
            "OrganizationId");

        migrationBuilder.CreateIndex(
            "IX_OrganizationMember_OrganizationId",
            "OrganizationMember",
            "OrganizationId");

        migrationBuilder.CreateIndex(
            "IX_OrganizationUsedName_OrganizationId",
            "OrganizationUsedName",
            "OrganizationId");

        migrationBuilder.CreateIndex(
            "IX_PasswordHistory_UserId",
            "PasswordHistory",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_PasswordHistory_WhenCreated",
            "PasswordHistory",
            "WhenCreated");

        migrationBuilder.CreateIndex(
            "IX_PersonBankAccount_PersonId",
            "PersonBankAccount",
            "PersonId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "JoinOrganizationInvitation");

        migrationBuilder.DropTable(
            "NaturalPersonClaim");

        migrationBuilder.DropTable(
            "NaturalPersonLogin");

        migrationBuilder.DropTable(
            "NaturalPersonToken");

        migrationBuilder.DropTable(
            "OrganizationBankAccount");

        migrationBuilder.DropTable(
            "OrganizationIdentifier");

        migrationBuilder.DropTable(
            "OrganizationMember");

        migrationBuilder.DropTable(
            "OrganizationUsedName");

        migrationBuilder.DropTable(
            "PasswordHistory");

        migrationBuilder.DropTable(
            "PersonBankAccount");

        migrationBuilder.DropTable(
            "Organization");

        migrationBuilder.DropTable(
            "NaturalPerson");
    }
}