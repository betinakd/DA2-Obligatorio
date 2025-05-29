using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InstancePolymorphism : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InstanceId",
                table: "SimAttributes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ReferenceId",
                table: "ParameterSignatures",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstanceId",
                table: "ParameterSignatures",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InstanceId",
                table: "LocalVariables",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SimAttributes_InstanceId",
                table: "SimAttributes",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSignatures_InstanceId",
                table: "ParameterSignatures",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalVariables_InstanceId",
                table: "LocalVariables",
                column: "InstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalVariables_SimClasses_InstanceId",
                table: "LocalVariables",
                column: "InstanceId",
                principalTable: "SimClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterSignatures_SimClasses_InstanceId",
                table: "ParameterSignatures",
                column: "InstanceId",
                principalTable: "SimClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SimAttributes_SimClasses_InstanceId",
                table: "SimAttributes",
                column: "InstanceId",
                principalTable: "SimClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalVariables_SimClasses_InstanceId",
                table: "LocalVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterSignatures_SimClasses_InstanceId",
                table: "ParameterSignatures");

            migrationBuilder.DropForeignKey(
                name: "FK_SimAttributes_SimClasses_InstanceId",
                table: "SimAttributes");

            migrationBuilder.DropIndex(
                name: "IX_SimAttributes_InstanceId",
                table: "SimAttributes");

            migrationBuilder.DropIndex(
                name: "IX_ParameterSignatures_InstanceId",
                table: "ParameterSignatures");

            migrationBuilder.DropIndex(
                name: "IX_LocalVariables_InstanceId",
                table: "LocalVariables");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "SimAttributes");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "ParameterSignatures");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "LocalVariables");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReferenceId",
                table: "ParameterSignatures",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
