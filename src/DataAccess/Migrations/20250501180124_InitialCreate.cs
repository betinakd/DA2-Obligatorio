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
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    { new Guid("22223222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), "bool", 2 },
                    { new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), new Guid("11111111-1111-1111-1111-111111111111"), "Motor", 2 },
                    { new Guid("a4a2a4a4-a4a4-a4a4-a4a4-a4a4a4a4a4a4"), new Guid("11111111-1111-1111-1111-111111111111"), "Validador", 2 },
                    { new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("11111111-1111-1111-1111-111111111111"), "Auto", 2 },
                    { new Guid("b4b424b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("11111111-1111-1111-1111-111111111111"), "Entrada", 2 }
                });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "ReferenceThis_ReferenceId", "ReferenceType", "RelatedInvocationId" },
                values: new object[,]
                {
                    { new Guid("c2c7c7c7-c7c7-c7c7-c7c7-c7c7c7c7c7c7"), new Guid("b4b424b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), "This", new Guid("d2d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6") },
                    { new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), "This", new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2") }
                });

            migrationBuilder.InsertData(
                table: "SimAttributes",
                columns: new[] { "Id", "Name", "Privacity", "RelatedClassId", "TypeId" },
                values: new object[] { new Guid("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"), "motorAuto", 0, new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1") });

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[] { new Guid("c4c4c2c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), new Guid("b4b424b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), "EntradaTexto", 2 });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturnTypeId" },
                values: new object[,]
                {
                    { new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), 2, "Encender", 2, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("c5c5c5c5-c5c5-c5c5-c2c5-c5c5c5c5c5c5"), 2, "Limpiar", 1, new Guid("b4b424b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"), 2, "Apagar", 2, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("d5d5d5d5-d5d5-d2d5-d5d5-d5d5d5d5d5d5"), 2, "Inicializar", 1, new Guid("b4b424b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), 2, "IniciarViaje", 2, new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("e4e2e4e4-e4e4-e4e4-e4e4-e4e4e4e4e4e4"), 2, "Validar", 2, new Guid("a4a2a4a4-a4a4-a4a4-a4a4-a4a4a4a4a4a4"), new Guid("22223222-2222-2222-2222-222222222222") },
                    { new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"), 2, "FinalizarViaje", 2, new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("f4f2f4f4-f4f4-f4f4-f4f4-f4f4f4f4f4f4"), 2, "EsValido", 2, new Guid("b4b424b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("22223222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "Invocations",
                columns: new[] { "Id", "ReferenceId", "RelatedMethodId", "SignatureId" },
                values: new object[,]
                {
                    { new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2"), new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), new Guid("a3a3a3a3-a3a3-a3a3-a3a3-a3a3a3a3a3a3") },
                    { new Guid("d2d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6"), new Guid("c2c7c7c7-c7c7-c7c7-c7c7-c7c7c7c7c7c7"), new Guid("f4f2f4f4-f4f4-f4f4-f4f4-f4f4f4f4f4f4"), new Guid("a2a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7") }
                });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "ReferenceId", "ReferenceType", "RelatedInvocationId" },
                values: new object[,]
                {
                    { new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"), "Attribute", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2") },
                    { new Guid("b8b8b8b8-b8b8-b8b8-b8b8-b8b8b8b8b8b3"), new Guid("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"), "Attribute", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2") }
                });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "ReferenceThis_ReferenceId", "ReferenceType", "RelatedInvocationId" },
                values: new object[] { new Guid("d2d7d7d7-d7d7-d7d7-d7d7-d7d7d7d7d7d7"), new Guid("c4c4c2c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), "This", new Guid("c2c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6") });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "ReferenceId", "ReferenceType", "RelatedInvocationId" },
                values: new object[] { new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"), new Guid("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"), "Attribute", new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2") });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "ReferenceBase_ReferenceId", "ReferenceType", "RelatedInvocationId" },
                values: new object[] { new Guid("e2e7e7e7-e7e7-e7e7-e7e7-e7e7e7e7e7e7"), new Guid("c4c4c2c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), "Base", new Guid("b2b6b6b6-b6b6-b6b6-b6b6-b6b6b6b6b6b6") });

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "State" },
                values: new object[] { new Guid("d4d2d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), new Guid("c4c4c2c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), "EntradaTextoEspecial", 0 });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturnTypeId" },
                values: new object[] { new Guid("a2a5a5a5-a5a5-a5a5-a5a5-a5a5a5a5a5a5"), 2, "EsValido", 2, new Guid("c4c4c2c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), new Guid("22223222-2222-2222-2222-222222222222") });

            migrationBuilder.InsertData(
                table: "Invocations",
                columns: new[] { "Id", "ReferenceId", "RelatedMethodId", "SignatureId" },
                values: new object[,]
                {
                    { new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2") },
                    { new Guid("b2b6b6b6-b6b6-b6b6-b6b6-b6b6b6b6b6b6"), new Guid("e2e7e7e7-e7e7-e7e7-e7e7-e7e7e7e7e7e7"), new Guid("a2a5a5a5-a5a5-a5a5-a5a5-a5a5a5a5a5a5"), new Guid("f6f3f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6") },
                    { new Guid("c2c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6"), new Guid("d2d7d7d7-d7d7-d7d7-d7d7-d7d7d7d7d7d7"), new Guid("a2a5a5a5-a5a5-a5a5-a5a5-a5a5a5a5a5a5"), new Guid("f6f2f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6") },
                    { new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"), new Guid("b8b8b8b8-b8b8-b8b8-b8b8-b8b8b8b8b8b3"), new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"), new Guid("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2") }
                });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "ReferenceBase_ReferenceId", "ReferenceType", "RelatedInvocationId" },
                values: new object[] { new Guid("f2f7f7f7-f7f7-f7f7-f7f7-f7f7f7f7f7f7"), new Guid("d4d2d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), "Base", new Guid("a2a6a6a6-a6a6-a6a6-a6a6-a6a6a6a6a6a6") });

            migrationBuilder.InsertData(
                table: "Signatures",
                columns: new[] { "Id", "Name", "RelatedInvocationId" },
                values: new object[,]
                {
                    { new Guid("a2a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7"), "Inicializar", new Guid("d2d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6") },
                    { new Guid("a3a3a3a3-a3a3-a3a3-a3a3-a3a3a3a3a3a3"), "IniciarViaje", new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2") }
                });

            migrationBuilder.InsertData(
                table: "SimAttributes",
                columns: new[] { "Id", "Name", "Privacity", "RelatedClassId", "TypeId" },
                values: new object[] { new Guid("e5e5e2e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), "input", 0, new Guid("a4a2a4a4-a4a4-a4a4-a4a4-a4a4a4a4a4a4"), new Guid("d4d2d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4") });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "Name", "Privacity", "RelatedClassId", "ReturnTypeId" },
                values: new object[] { new Guid("b2b5b5b5-b5b5-b5b5-b5b5-b5b5b5b5b5b5"), 0, "EsValido", 2, new Guid("d4d2d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), new Guid("22223222-2222-2222-2222-222222222222") });

            migrationBuilder.InsertData(
                table: "Invocations",
                columns: new[] { "Id", "ReferenceId", "RelatedMethodId", "SignatureId" },
                values: new object[] { new Guid("a2a6a6a6-a6a6-a6a6-a6a6-a6a6a6a6a6a6"), new Guid("f2f7f7f7-f7f7-f7f7-f7f7-f7f7f7f7f7f7"), new Guid("b2b5b5b5-b5b5-b5b5-b5b5-b5b5b5b5b5b5"), new Guid("e6e3e6e6-e6e6-e6e6-e6e6-e6e6e6e6e6e6") });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "ReferenceId", "ReferenceType", "RelatedInvocationId" },
                values: new object[] { new Guid("b7b2b7b7-b7b7-b7b7-b7b7-b7b7b7b7b7b7"), new Guid("e5e5e2e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), "Attribute", new Guid("f2f5f5f5-f5f5-f5f5-f5f5-f5f5f5f5f5f5") });

            migrationBuilder.InsertData(
                table: "Signatures",
                columns: new[] { "Id", "Name", "RelatedInvocationId" },
                values: new object[,]
                {
                    { new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), "Encender", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2") },
                    { new Guid("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2"), "Apagar", new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2") },
                    { new Guid("f6f2f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"), "Limpiar", new Guid("c2c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6") },
                    { new Guid("f6f3f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"), "EsValido", new Guid("b2b6b6b6-b6b6-b6b6-b6b6-b6b6b6b6b6b6") }
                });

            migrationBuilder.InsertData(
                table: "Invocations",
                columns: new[] { "Id", "ReferenceId", "RelatedMethodId", "SignatureId" },
                values: new object[] { new Guid("f2f5f5f5-f5f5-f5f5-f5f5-f5f5f5f5f5f5"), new Guid("b7b2b7b7-b7b7-b7b7-b7b7-b7b7b7b7b7b7"), new Guid("e4e2e4e4-e4e4-e4e4-e4e4-e4e4e4e4e4e4"), new Guid("e6e2e6e6-e6e6-e6e6-e6e6-e6e6e6e6e6e6") });

            migrationBuilder.InsertData(
                table: "Signatures",
                columns: new[] { "Id", "Name", "RelatedInvocationId" },
                values: new object[,]
                {
                    { new Guid("e6e3e6e6-e6e6-e6e6-e6e6-e6e6e6e6e6e6"), "EsValido", new Guid("a2a6a6a6-a6a6-a6a6-a6a6-a6a6a6a6a6a6") },
                    { new Guid("e6e2e6e6-e6e6-e6e6-e6e6-e6e6e6e6e6e6"), "EsValido", new Guid("f2f5f5f5-f5f5-f5f5-f5f5-f5f5f5f5f5f5") }
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
