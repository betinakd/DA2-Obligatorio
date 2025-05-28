using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class virtualoverride : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOverride",
                table: "SimMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVirtual",
                table: "SimMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                table: "Parameters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                table: "LocalVariables",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-1111-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-2222-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-3333-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-4444-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-6666-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-7777-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-8888-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { false, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOverride",
                table: "SimMethods");

            migrationBuilder.DropColumn(
                name: "IsVirtual",
                table: "SimMethods");

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                table: "Parameters",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                table: "LocalVariables",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
