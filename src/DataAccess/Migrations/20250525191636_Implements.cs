using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Implements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimClassImplements",
                columns: table => new
                {
                    ImplementsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimClassImplements", x => new { x.ImplementsId, x.SimClassId });
                    table.ForeignKey(
                        name: "FK_SimClassImplements_SimClasses_ImplementsId",
                        column: x => x.ImplementsId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimClassImplements_SimClasses_SimClassId",
                        column: x => x.SimClassId,
                        principalTable: "SimClasses",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "BaseClassId",
                value: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.CreateIndex(
                name: "IX_SimClassImplements_SimClassId",
                table: "SimClassImplements",
                column: "SimClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimClassImplements");

            migrationBuilder.UpdateData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "BaseClassId",
                value: null);
        }
    }
}
