using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("1f1c058f-d16f-4026-ad14-cfaf5dacd162"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("69944855-67ad-47dc-9950-6c653b9d29eb"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("6d0b9c8e-4cc7-472c-9835-c9ee7456317c"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("ada0ea3d-1087-47db-a128-8015e0c966e6"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("ba2feee1-2f3d-4b90-af73-6d949b4ac453"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("dd58c2f0-4606-4fff-ad0b-0ea5b8808c08"));

            migrationBuilder.DeleteData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("b2320cd7-26f5-4503-b319-1919459a21bf"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("ead16992-fbcd-44d6-a92c-db535c962582"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"));

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("504f38cd-996c-4845-a703-5a449050c775"), null, "Object", 2 },
                    { new Guid("0ea21daa-75d4-4072-ad0c-97cbf5e806bd"), new Guid("504f38cd-996c-4845-a703-5a449050c775"), "string", 2 },
                    { new Guid("2f6e81fd-d9f3-4b3b-8495-18db97a180d8"), new Guid("504f38cd-996c-4845-a703-5a449050c775"), "decimal", 2 },
                    { new Guid("3a7c1a39-708a-408d-b360-5ac1b9e77ef5"), new Guid("504f38cd-996c-4845-a703-5a449050c775"), "bool", 2 },
                    { new Guid("b5467b9b-29be-4986-ac3f-b69c6116f0ff"), new Guid("504f38cd-996c-4845-a703-5a449050c775"), "char", 2 },
                    { new Guid("db3531f5-2430-4ef5-a07c-158e217c0d40"), new Guid("504f38cd-996c-4845-a703-5a449050c775"), "float", 2 },
                    { new Guid("dc1f15a3-65b3-4f5b-a4bd-a19e51c95038"), new Guid("504f38cd-996c-4845-a703-5a449050c775"), "double", 2 },
                    { new Guid("f2bdcb72-86b4-4546-9b98-94ee3e6f6862"), new Guid("504f38cd-996c-4845-a703-5a449050c775"), "int", 2 }
                });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturTypeId" },
                values: new object[] { new Guid("b48d1779-02c9-4048-999d-230d7e66b978"), 2, "Equals", 0, new Guid("504f38cd-996c-4845-a703-5a449050c775"), new Guid("3a7c1a39-708a-408d-b360-5ac1b9e77ef5") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("0ea21daa-75d4-4072-ad0c-97cbf5e806bd"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("2f6e81fd-d9f3-4b3b-8495-18db97a180d8"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("b5467b9b-29be-4986-ac3f-b69c6116f0ff"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("db3531f5-2430-4ef5-a07c-158e217c0d40"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("dc1f15a3-65b3-4f5b-a4bd-a19e51c95038"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("f2bdcb72-86b4-4546-9b98-94ee3e6f6862"));

            migrationBuilder.DeleteData(
                table: "SimMethods",
                keyColumn: "Id",
                keyValue: new Guid("b48d1779-02c9-4048-999d-230d7e66b978"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("3a7c1a39-708a-408d-b360-5ac1b9e77ef5"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("504f38cd-996c-4845-a703-5a449050c775"));

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), null, "Object", 2 },
                    { new Guid("1f1c058f-d16f-4026-ad14-cfaf5dacd162"), new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), "double", 2 },
                    { new Guid("69944855-67ad-47dc-9950-6c653b9d29eb"), new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), "float", 2 },
                    { new Guid("6d0b9c8e-4cc7-472c-9835-c9ee7456317c"), new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), "string", 2 },
                    { new Guid("ada0ea3d-1087-47db-a128-8015e0c966e6"), new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), "char", 2 },
                    { new Guid("ba2feee1-2f3d-4b90-af73-6d949b4ac453"), new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), "int", 2 },
                    { new Guid("dd58c2f0-4606-4fff-ad0b-0ea5b8808c08"), new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), "decimal", 2 },
                    { new Guid("ead16992-fbcd-44d6-a92c-db535c962582"), new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), "bool", 2 }
                });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturTypeId" },
                values: new object[] { new Guid("b2320cd7-26f5-4503-b319-1919459a21bf"), 2, "Equals", 0, new Guid("8ffcbe9b-8013-434c-b430-c910f791e602"), new Guid("ead16992-fbcd-44d6-a92c-db535c962582") });
        }
    }
}
