using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class methods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("3e9732a1-5cf3-4e01-a198-29e0aafd2775"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("4aae14ba-bc64-49c3-a6a2-782fda18b505"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("6aa0635e-f815-4b98-a5bb-2155db925818"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("7bcdad3a-bf20-49e9-add3-2504adbffc38"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("a4527222-3dfe-4416-b8aa-69b06784438a"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("d2a420f6-b2e0-45ed-b4a1-6b2f3893e740"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("fffc5c56-744e-49f9-8940-24e5d308785f"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"));

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), null, "Object", 2 },
                    { new Guid("121253a8-be57-4a7e-b751-68d6696d3f05"), new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), "string", 2 },
                    { new Guid("36e7f680-9d9f-4f15-9db5-f2c0808ab5f7"), new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), "double", 2 },
                    { new Guid("7346ca9c-4144-49eb-ad25-34abc85f3360"), new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), "decimal", 2 },
                    { new Guid("77a34eb4-1691-4fbf-8792-7b19706a2551"), new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), "bool", 2 },
                    { new Guid("84cd78ef-9651-4038-bc20-6ccde806a2dc"), new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), "int", 2 },
                    { new Guid("9f08665b-5498-4c3e-bf48-ab52ac3f762f"), new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), "float", 2 },
                    { new Guid("f9bc54fe-b086-4880-a9de-66df85840dce"), new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"), "char", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("121253a8-be57-4a7e-b751-68d6696d3f05"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("36e7f680-9d9f-4f15-9db5-f2c0808ab5f7"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("7346ca9c-4144-49eb-ad25-34abc85f3360"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("77a34eb4-1691-4fbf-8792-7b19706a2551"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("84cd78ef-9651-4038-bc20-6ccde806a2dc"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("9f08665b-5498-4c3e-bf48-ab52ac3f762f"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("f9bc54fe-b086-4880-a9de-66df85840dce"));

            migrationBuilder.DeleteData(
                table: "SimClasses",
                keyColumn: "Id",
                keyValue: new Guid("9f638d34-4914-4fae-9d74-4ef675e72357"));

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), null, "Object", 2 },
                    { new Guid("3e9732a1-5cf3-4e01-a198-29e0aafd2775"), new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), "decimal", 2 },
                    { new Guid("4aae14ba-bc64-49c3-a6a2-782fda18b505"), new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), "float", 2 },
                    { new Guid("6aa0635e-f815-4b98-a5bb-2155db925818"), new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), "char", 2 },
                    { new Guid("7bcdad3a-bf20-49e9-add3-2504adbffc38"), new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), "bool", 2 },
                    { new Guid("a4527222-3dfe-4416-b8aa-69b06784438a"), new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), "int", 2 },
                    { new Guid("d2a420f6-b2e0-45ed-b4a1-6b2f3893e740"), new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), "double", 2 },
                    { new Guid("fffc5c56-744e-49f9-8940-24e5d308785f"), new Guid("735baeae-0889-481f-8eb6-b85a4d16959f"), "string", 2 }
                });
        }
    }
}
