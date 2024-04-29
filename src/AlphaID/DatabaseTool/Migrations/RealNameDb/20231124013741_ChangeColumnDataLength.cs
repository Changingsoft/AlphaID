#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseTool.Migrations.RealNameDb;

/// <inheritdoc />
public partial class ChangeColumnDataLength : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            "Name",
            "RealNameRequest",
            "nvarchar(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Issuer",
            "RealNameRequest",
            "nvarchar(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Ethnicity",
            "RealNameRequest",
            "nvarchar(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "CardNumber",
            "RealNameRequest",
            "varchar(18)",
            false,
            18,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Auditor",
            "RealNameRequest",
            "nvarchar(30)",
            maxLength: 30,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Address",
            "RealNameRequest",
            "nvarchar(150)",
            maxLength: 150,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "ValidatedBy",
            "RealNameAuthentication",
            "nvarchar(30)",
            maxLength: 30,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            "Name",
            "RealNameRequest",
            "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Issuer",
            "RealNameRequest",
            "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Ethnicity",
            "RealNameRequest",
            "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "CardNumber",
            "RealNameRequest",
            "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(18)",
            oldUnicode: false,
            oldMaxLength: 18,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Auditor",
            "RealNameRequest",
            "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(30)",
            oldMaxLength: 30,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "Address",
            "RealNameRequest",
            "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(150)",
            oldMaxLength: 150,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            "ValidatedBy",
            "RealNameAuthentication",
            "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(30)",
            oldMaxLength: 30);
    }
}