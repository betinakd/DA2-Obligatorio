using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class StaticReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStatic",
                table: "SimMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStatic",
                table: "SimAttributes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceType",
                table: "References",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceStaticAttribute_ReferenceId",
                table: "References",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceStatic_ReferenceId",
                table: "References",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-1111-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-2222-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-3333-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-4444-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-6666-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-7777-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-8888-1111-1111-111111111111"),
                column: "IsStatic",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceStatic_ReferenceId",
                table: "References",
                column: "ReferenceStatic_ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceStaticAttribute_ReferenceId",
                table: "References",
                column: "ReferenceStaticAttribute_ReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_References_SimAttributes_ReferenceStaticAttribute_ReferenceId",
                table: "References",
                column: "ReferenceStaticAttribute_ReferenceId",
                principalTable: "SimAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_References_SimClasses_ReferenceStatic_ReferenceId",
                table: "References",
                column: "ReferenceStatic_ReferenceId",
                principalTable: "SimClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_References_SimAttributes_ReferenceStaticAttribute_ReferenceId",
                table: "References");

            migrationBuilder.DropForeignKey(
                name: "FK_References_SimClasses_ReferenceStatic_ReferenceId",
                table: "References");

            migrationBuilder.DropIndex(
                name: "IX_References_ReferenceStatic_ReferenceId",
                table: "References");

            migrationBuilder.DropIndex(
                name: "IX_References_ReferenceStaticAttribute_ReferenceId",
                table: "References");

            migrationBuilder.DropColumn(
                name: "IsStatic",
                table: "SimMethods");

            migrationBuilder.DropColumn(
                name: "IsStatic",
                table: "SimAttributes");

            migrationBuilder.DropColumn(
                name: "ReferenceStaticAttribute_ReferenceId",
                table: "References");

            migrationBuilder.DropColumn(
                name: "ReferenceStatic_ReferenceId",
                table: "References");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceType",
                table: "References",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
