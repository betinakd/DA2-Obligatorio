using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class me : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
