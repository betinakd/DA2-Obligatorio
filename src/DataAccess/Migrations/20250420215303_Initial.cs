using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    RelatedClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimAttributes_SimClasses_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SimMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                        name: "FK_SimMethods_SimClasses_ReturnTypeId",
                        column: x => x.ReturnTypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id");
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LocalVariables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        principalColumn: "Id");
                });

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
                name: "IX_SimMethods_ReturnTypeId",
                table: "SimMethods",
                column: "ReturnTypeId");
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
