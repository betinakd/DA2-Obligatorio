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
                name: "ExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Execution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectCreate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Privacity = table.Column<int>(type: "int", nullable: true),
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
                    ReturnTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalVariables_SimMethods_RelatedMethodId",
                        column: x => x.RelatedMethodId,
                        principalTable: "SimMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_SimClasses_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parameters_SimMethods_RelatedMethodId",
                        column: x => x.RelatedMethodId,
                        principalTable: "SimMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "References",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedInvocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceBase_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceParameter_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceThis_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceVariable_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_References", x => x.Id);
                    table.ForeignKey(
                        name: "FK_References_LocalVariables_ReferenceVariable_ReferenceId",
                        column: x => x.ReferenceVariable_ReferenceId,
                        principalTable: "LocalVariables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_References_Parameters_ReferenceParameter_ReferenceId",
                        column: x => x.ReferenceParameter_ReferenceId,
                        principalTable: "Parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_References_SimAttributes_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "SimAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_References_SimClasses_ReferenceBase_ReferenceId",
                        column: x => x.ReferenceBase_ReferenceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_References_SimClasses_ReferenceThis_ReferenceId",
                        column: x => x.ReferenceThis_ReferenceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invocations_References_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "References",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invocations_SimMethods_RelatedMethodId",
                        column: x => x.RelatedMethodId,
                        principalTable: "SimMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedInvocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signatures_Invocations_RelatedInvocationId",
                        column: x => x.RelatedInvocationId,
                        principalTable: "Invocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParameterSignatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParameterSignatures_Signatures_SignatureId",
                        column: x => x.SignatureId,
                        principalTable: "Signatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParameterSignatures_SimClasses_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, "Object", 2 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), "void", 2 },
                    { new Guid("22223222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), "bool", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invocations_ReferenceId",
                table: "Invocations",
                column: "ReferenceId",
                unique: true,
                filter: "[ReferenceId] IS NOT NULL");

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
                name: "IX_Parameters_RelatedMethodId",
                table: "Parameters",
                column: "RelatedMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_TypeId",
                table: "Parameters",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSignatures_SignatureId",
                table: "ParameterSignatures",
                column: "SignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSignatures_TypeId",
                table: "ParameterSignatures",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceBase_ReferenceId",
                table: "References",
                column: "ReferenceBase_ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceId",
                table: "References",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceParameter_ReferenceId",
                table: "References",
                column: "ReferenceParameter_ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceThis_ReferenceId",
                table: "References",
                column: "ReferenceThis_ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceVariable_ReferenceId",
                table: "References",
                column: "ReferenceVariable_ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_RelatedInvocationId",
                table: "Signatures",
                column: "RelatedInvocationId",
                unique: true);

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
                name: "ExecutionLogs");

            migrationBuilder.DropTable(
                name: "ParameterSignatures");

            migrationBuilder.DropTable(
                name: "Signatures");

            migrationBuilder.DropTable(
                name: "Invocations");

            migrationBuilder.DropTable(
                name: "References");

            migrationBuilder.DropTable(
                name: "LocalVariables");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "SimAttributes");

            migrationBuilder.DropTable(
                name: "SimMethods");

            migrationBuilder.DropTable(
                name: "SimClasses");
        }
    }
}
