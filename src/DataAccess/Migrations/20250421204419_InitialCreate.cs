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
                    { new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), null, "Object", 2 },
                    { new Guid("155777ae-6e2e-4137-9019-a7e909f4184c"), new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), "decimal", 2 },
                    { new Guid("1ac3a574-fcdf-4ecf-9a0c-7d5a181fae19"), new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), "float", 2 },
                    { new Guid("a101c08b-80fe-4123-9d06-bb61fc6e5d6e"), new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), "string", 2 },
                    { new Guid("a3bd3704-226e-40a2-921e-f0f29bfc61a8"), new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), "bool", 2 },
                    { new Guid("b5c3d936-17a1-4466-8079-ab14ae01b17a"), new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), "char", 2 },
                    { new Guid("c6df8671-e785-4f72-b2fa-93c1ebef1aa2"), new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), "double", 2 },
                    { new Guid("ef81f642-4a4a-4423-88c1-d81f8f2a5843"), new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), "int", 2 }
                });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturTypeId" },
                values: new object[] { new Guid("14bd333c-406a-4e1a-bb77-44a898d92c7d"), 2, "Equals", 0, new Guid("99a4b484-f05b-4758-a15f-e1170ea00fd0"), new Guid("a3bd3704-226e-40a2-921e-f0f29bfc61a8") });

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
