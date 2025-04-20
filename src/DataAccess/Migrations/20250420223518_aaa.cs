using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class aaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invocations_SimMethods_RelatedMethodId",
                table: "Invocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Parameters_SimMethods_RelatedMethodId",
                table: "Parameters");

            migrationBuilder.DropForeignKey(
                name: "FK_SimAttributes_SimClasses_TypeId",
                table: "SimAttributes");

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("5ab44918-af09-4258-8457-7148c8d6125a"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("5fd5124d-960e-48af-81b8-ec7d5339fada"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("822281e2-8681-47fd-bcef-c47e978823fd"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("a5bdcf9a-d109-43bd-b290-4bdc0f046fb0"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("b5a5d168-3db3-408e-90a8-b860bf7f582d"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("bffc09e6-1c0e-4c5b-a0b4-b23896bfc8eb"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("c307917c-828c-45f6-80d7-feb5c4f3b532"));

            migrationBuilder.DeleteData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("d3c159ab-4b74-491f-98df-54eff0756581"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"));

            migrationBuilder.AlterColumn<Guid>(
                name: "RelatedMethodId",
                table: "LocalVariables",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), null, "Object", 2 },
                    { new Guid("083aa66d-50d6-48ea-b0d2-17376e66c072"), new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), "decimal", 2 },
                    { new Guid("1a77b10e-c125-4f2f-808c-286546e2afba"), new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), "bool", 2 },
                    { new Guid("465ea3e8-af4f-4f03-adf7-c8003c30a259"), new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), "double", 2 },
                    { new Guid("51e7d7df-c95c-4e93-ad81-2d6371aaf7a8"), new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), "float", 2 },
                    { new Guid("6410546b-ab34-4168-9d5e-c434400dcef2"), new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), "string", 2 },
                    { new Guid("6bcaef21-441d-4d61-a8c8-96c9ac3c9cd2"), new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), "int", 2 },
                    { new Guid("e05387be-86de-4edd-bb56-1624fb55439e"), new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), "char", 2 }
                });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturnTypeId" },
                values: new object[] { new Guid("bbe54e44-2ccd-4c0e-9379-d5ac3cc5f0b2"), 2, "Equals", 0, new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"), null });

            migrationBuilder.AddForeignKey(
                name: "FK_Invocations_SimMethods_RelatedMethodId",
                table: "Invocations",
                column: "RelatedMethodId",
                principalTable: "SimMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Parameters_SimMethods_RelatedMethodId",
                table: "Parameters",
                column: "RelatedMethodId",
                principalTable: "SimMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimAttributes_SimClasses_TypeId",
                table: "SimAttributes",
                column: "TypeId",
                principalTable: "SimClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invocations_SimMethods_RelatedMethodId",
                table: "Invocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Parameters_SimMethods_RelatedMethodId",
                table: "Parameters");

            migrationBuilder.DropForeignKey(
                name: "FK_SimAttributes_SimClasses_TypeId",
                table: "SimAttributes");

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("083aa66d-50d6-48ea-b0d2-17376e66c072"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("1a77b10e-c125-4f2f-808c-286546e2afba"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("465ea3e8-af4f-4f03-adf7-c8003c30a259"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("51e7d7df-c95c-4e93-ad81-2d6371aaf7a8"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("6410546b-ab34-4168-9d5e-c434400dcef2"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("6bcaef21-441d-4d61-a8c8-96c9ac3c9cd2"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("e05387be-86de-4edd-bb56-1624fb55439e"));

            migrationBuilder.DeleteData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("bbe54e44-2ccd-4c0e-9379-d5ac3cc5f0b2"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("450360bc-385d-4d28-8f4b-74fb804bd34d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "RelatedMethodId",
                table: "LocalVariables",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), null, "Object", 2 },
                    { new Guid("5ab44918-af09-4258-8457-7148c8d6125a"), new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), "decimal", 2 },
                    { new Guid("5fd5124d-960e-48af-81b8-ec7d5339fada"), new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), "string", 2 },
                    { new Guid("822281e2-8681-47fd-bcef-c47e978823fd"), new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), "double", 2 },
                    { new Guid("a5bdcf9a-d109-43bd-b290-4bdc0f046fb0"), new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), "float", 2 },
                    { new Guid("b5a5d168-3db3-408e-90a8-b860bf7f582d"), new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), "bool", 2 },
                    { new Guid("bffc09e6-1c0e-4c5b-a0b4-b23896bfc8eb"), new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), "char", 2 },
                    { new Guid("c307917c-828c-45f6-80d7-feb5c4f3b532"), new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), "int", 2 }
                });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturnTypeId" },
                values: new object[] { new Guid("d3c159ab-4b74-491f-98df-54eff0756581"), 2, "Equals", 0, new Guid("4c260b10-9e53-4db2-b195-8124fc32bf36"), null });

            migrationBuilder.AddForeignKey(
                name: "FK_Invocations_SimMethods_RelatedMethodId",
                table: "Invocations",
                column: "RelatedMethodId",
                principalTable: "SimMethods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parameters_SimMethods_RelatedMethodId",
                table: "Parameters",
                column: "RelatedMethodId",
                principalTable: "SimMethods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SimAttributes_SimClasses_TypeId",
                table: "SimAttributes",
                column: "TypeId",
                principalTable: "SimClasses",
                principalColumn: "Id");
        }
    }
}
