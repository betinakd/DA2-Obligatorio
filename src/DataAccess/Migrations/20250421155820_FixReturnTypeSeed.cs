using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixReturnTypeSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimClasses_SimClasses_BaseClassId",
                        column: x => x.BaseClassId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SimAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Privacity = table.Column<int>(type: "int", nullable: false),
                    RelatedClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimAttributes_SimClasses_RelatedClassId",
                        column: x => x.RelatedClassId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SimAttributes_SimClasses_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SimMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Privacity = table.Column<int>(type: "int", nullable: false),
                    Accesibility = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimMethods_SimClasses_RelatedClassId",
                        column: x => x.RelatedClassId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimMethods_SimClasses_ReturTypeId",
                        column: x => x.ReturTypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MethodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invocations_SimMethods_RelatedMethodId",
                        column: x => x.RelatedMethodId,
                        principalTable: "SimMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalVariables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalVariables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalVariables_SimClasses_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocalVariables_SimMethods_RelatedMethodId",
                        column: x => x.RelatedMethodId,
                        principalTable: "SimMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_Invocations_InvocationId",
                        column: x => x.InvocationId,
                        principalTable: "Invocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parameters_SimClasses_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parameters_SimMethods_RelatedMethodId",
                        column: x => x.RelatedMethodId,
                        principalTable: "SimMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Invocations_RelatedMethodId",
                table: "Invocations",
                column: "RelatedMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalVariables_RelatedMethodId",
                table: "LocalVariables",
                column: "RelatedMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalVariables_TypeId",
                table: "LocalVariables",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_InvocationId",
                table: "Parameters",
                column: "InvocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_RelatedMethodId",
                table: "Parameters",
                column: "RelatedMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_TypeId",
                table: "Parameters",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimAttributes_RelatedClassId",
                table: "SimAttributes",
                column: "RelatedClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SimAttributes_TypeId",
                table: "SimAttributes",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimClasses_BaseClassId",
                table: "SimClasses",
                column: "BaseClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SimMethods_RelatedClassId",
                table: "SimMethods",
                column: "RelatedClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SimMethods_ReturTypeId",
                table: "SimMethods",
                column: "ReturTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalVariables");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "SimAttributes");

            migrationBuilder.DropTable(
                name: "Invocations");

            migrationBuilder.DropTable(
                name: "SimMethods");

            migrationBuilder.DropTable(
                name: "SimClasses");
        }
    }
}
