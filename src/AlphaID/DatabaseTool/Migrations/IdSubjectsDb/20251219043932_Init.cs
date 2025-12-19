using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseTool.Migrations.IdSubjectsDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneticSurname = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PhoneticGivenName = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    PersonWhenChanged = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SearchHint = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    SecurityStamp = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    PasswordLastSet = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    WhenCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WhenChanged = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GivenName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Gender = table.Column<string>(type: "varchar(6)", nullable: true, comment: "性别"),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    ProfilePicture_MimeType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ProfilePicture_Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Locale = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    TimeZone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Address_Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_Locality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_Street1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_Street2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_Street3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_Recipient = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_Contact = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address_PostalCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    WebSite = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "varchar(50)", nullable: false),
                    ClaimType = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserClaim_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_ApplicationUserLogin_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_ApplicationUserToken_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NaturalPersonBankAccount",
                columns: table => new
                {
                    AccountNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NaturalPersonId = table.Column<string>(type: "varchar(50)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturalPersonBankAccount", x => new { x.AccountNumber, x.NaturalPersonId });
                    table.ForeignKey(
                        name: "FK_NaturalPersonBankAccount_ApplicationUser_NaturalPersonId",
                        column: x => x.NaturalPersonId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ApplicationUserInRole",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RoleId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserInRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserInRole_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserInRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "varchar(50)", nullable: false),
                    ClaimType = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "ApplicationUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Email",
                table: "ApplicationUser",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Name",
                table: "ApplicationUser",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_SearchHint",
                table: "ApplicationUser",
                column: "SearchHint");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_UserName",
                table: "ApplicationUser",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_WhenChanged",
                table: "ApplicationUser",
                column: "WhenChanged");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_WhenCreated",
                table: "ApplicationUser",
                column: "WhenCreated");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ApplicationUser",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserClaim_UserId",
                table: "ApplicationUserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserInRole_RoleId",
                table: "ApplicationUserInRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLogin_UserId",
                table: "ApplicationUserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NaturalPersonBankAccount_NaturalPersonId",
                table: "NaturalPersonBankAccount",
                column: "NaturalPersonId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_RoleId",
                table: "RoleClaim",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserClaim");

            migrationBuilder.DropTable(
                name: "ApplicationUserInRole");

            migrationBuilder.DropTable(
                name: "ApplicationUserLogin");

            migrationBuilder.DropTable(
                name: "ApplicationUserToken");

            migrationBuilder.DropTable(
                name: "NaturalPersonBankAccount");

            migrationBuilder.DropTable(
                name: "RoleClaim");

            migrationBuilder.DropTable(
                name: "UsedPassword");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "ApplicationUser");
        }
    }
}
