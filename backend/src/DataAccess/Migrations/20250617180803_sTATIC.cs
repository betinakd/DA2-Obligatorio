using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sTATIC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_References_SimAttributes_ReferenceStaticAttribute_ReferenceId",
                table: "References");

            migrationBuilder.DropForeignKey(
                name: "FK_References_SimClasses_ReferenceStatic_ReferenceId",
                table: "References");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SimNamespaces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_References_SimAttributes_ReferenceStaticAttribute_ReferenceId",
                table: "References",
                column: "ReferenceStaticAttribute_ReferenceId",
                principalTable: "SimAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_References_SimClasses_ReferenceStatic_ReferenceId",
                table: "References",
                column: "ReferenceStatic_ReferenceId",
                principalTable: "SimClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SimNamespaces",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
    }
}
