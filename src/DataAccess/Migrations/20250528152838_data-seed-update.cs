using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dataseedupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-1111-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-2222-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-3333-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-4444-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-6666-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-7777-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("55555555-8888-1111-1111-111111111111"),
                columns: new[] { "IsOverride", "IsVirtual" },
                values: new object[] { true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
