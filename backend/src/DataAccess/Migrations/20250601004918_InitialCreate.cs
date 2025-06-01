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
                name: "ApiKeys",
                columns: table => new
                {
                    KeyValue = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.KeyValue);
                });

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
                name: "SimNamespaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseNamespaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimNamespaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimNamespaces_SimNamespaces_BaseNamespaceId",
                        column: x => x.BaseNamespaceId,
                        principalTable: "SimNamespaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SimClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NamespaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BaseClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_SimClasses_SimNamespaces_NamespaceId",
                        column: x => x.NamespaceId,
                        principalTable: "SimNamespaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SimAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Privacity = table.Column<int>(type: "int", nullable: true),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    RelatedClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimAttributes_SimClasses_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SimAttributes_SimClasses_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SimAttributes_SimClasses_RelatedClassId",
                        column: x => x.RelatedClassId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SimClassImplements",
                columns: table => new
                {
                    ImplementsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimClassImplements", x => new { x.ImplementsId, x.SimClassId });
                    table.ForeignKey(
                        name: "FK_SimClassImplements_SimClasses_ImplementsId",
                        column: x => x.ImplementsId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimClassImplements_SimClasses_SimClassId",
                        column: x => x.SimClassId,
                        principalTable: "SimClasses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SimMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Privacity = table.Column<int>(type: "int", nullable: false),
                    Accesibility = table.Column<int>(type: "int", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    IsOverride = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalVariables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalVariables_SimClasses_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocalVariables_SimClasses_ReferenceId",
                        column: x => x.ReferenceId,
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
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_SimClasses_ReferenceId",
                        column: x => x.ReferenceId,
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
                    ReferenceType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceBase_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceParameter_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceStatic_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceStaticAttribute_ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                        name: "FK_References_SimAttributes_ReferenceStaticAttribute_ReferenceId",
                        column: x => x.ReferenceStaticAttribute_ReferenceId,
                        principalTable: "SimAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_References_SimClasses_ReferenceBase_ReferenceId",
                        column: x => x.ReferenceBase_ReferenceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_References_SimClasses_ReferenceStatic_ReferenceId",
                        column: x => x.ReferenceStatic_ReferenceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        name: "FK_ParameterSignatures_SimClasses_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParameterSignatures_SimClasses_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "SimClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ApiKeys",
                columns: new[] { "KeyValue", "Name" },
                values: new object[,]
                {
                    { new Guid("77777777-aaaa-1111-1111-111111111111"), "validKey_1" },
                    { new Guid("77777777-bbbb-1111-1111-111111111111"), "validKey_2" }
                });

            migrationBuilder.InsertData(
                table: "SimNamespaces",
                columns: new[] { "Id", "BaseNamespaceId", "Name" },
                values: new object[] { new Guid("00000000-1111-0000-0000-000000000001"), null, "System" });

            migrationBuilder.InsertData(
                table: "SimClasses",
                columns: new[] { "Id", "BaseClassId", "Name", "NamespaceId", "State" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, "Object", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), "void", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("22223222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), "bool", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "byte", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-2222-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "sbyte", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-3333-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "char", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-4444-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "decimal", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-5555-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "double", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-6666-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "float", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-7777-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "int", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-8888-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "uint", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-9999-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "nint", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-aaaa-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "nuint", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-bbbb-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "long", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-cccc-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "ulong", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-dddd-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "short", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("33333333-eeee-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "ushort", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("44444444-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "string", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("44444444-2222-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "delegate", new Guid("00000000-1111-0000-0000-000000000001"), 2 },
                    { new Guid("44444444-3333-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "dynamic", new Guid("00000000-1111-0000-0000-000000000001"), 2 }
                });

            migrationBuilder.InsertData(
                table: "SimMethods",
                columns: new[] { "Id", "Accesibility", "IsOverride", "IsStatic", "IsVirtual", "Name", "Privacity", "RelatedClassId", "ReturnTypeId" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-1111-1111-111111111111"), 2, true, false, true, "GetType", 2, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("55555555-6666-1111-1111-111111111111"), 2, true, false, true, "MemberwiseClone", 1, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("55555555-1111-1111-1111-111111111111"), 2, true, false, true, "Equals", 2, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22223222-2222-2222-2222-222222222222") },
                    { new Guid("55555555-2222-1111-1111-111111111111"), 2, true, false, true, "Equals", 2, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22223222-2222-2222-2222-222222222222") },
                    { new Guid("55555555-3333-1111-1111-111111111111"), 2, true, false, true, "Finalize", 1, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("55555555-4444-1111-1111-111111111111"), 2, true, false, true, "GetHashCode", 2, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("33333333-7777-1111-1111-111111111111") },
                    { new Guid("55555555-7777-1111-1111-111111111111"), 2, true, false, true, "ReferenceEquals", 2, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22223222-2222-2222-2222-222222222222") },
                    { new Guid("55555555-8888-1111-1111-111111111111"), 2, true, false, true, "ToString", 2, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("44444444-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "Name", "ReferenceId", "RelatedMethodId" },
                values: new object[,]
                {
                    { new Guid("66666666-1111-1111-1111-111111111111"), "obj", new Guid("11111111-1111-1111-1111-111111111111"), new Guid("55555555-1111-1111-1111-111111111111") },
                    { new Guid("66666666-2222-1111-1111-111111111111"), "objA", new Guid("11111111-1111-1111-1111-111111111111"), new Guid("55555555-2222-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "Index", "Name", "ReferenceId", "RelatedMethodId" },
                values: new object[] { new Guid("66666666-3333-1111-1111-111111111111"), 1, "objB", new Guid("11111111-1111-1111-1111-111111111111"), new Guid("55555555-2222-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "Name", "ReferenceId", "RelatedMethodId" },
                values: new object[] { new Guid("66666666-4444-1111-1111-111111111111"), "objA", new Guid("11111111-1111-1111-1111-111111111111"), new Guid("55555555-7777-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "Index", "Name", "ReferenceId", "RelatedMethodId" },
                values: new object[] { new Guid("66666666-5555-1111-1111-111111111111"), 1, "objB", new Guid("11111111-1111-1111-1111-111111111111"), new Guid("55555555-7777-1111-1111-111111111111") });

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
                name: "IX_LocalVariables_InstanceId",
                table: "LocalVariables",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalVariables_ReferenceId",
                table: "LocalVariables",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalVariables_RelatedMethodId",
                table: "LocalVariables",
                column: "RelatedMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_ReferenceId",
                table: "Parameters",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_RelatedMethodId",
                table: "Parameters",
                column: "RelatedMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSignatures_InstanceId",
                table: "ParameterSignatures",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSignatures_ReferenceId",
                table: "ParameterSignatures",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSignatures_SignatureId",
                table: "ParameterSignatures",
                column: "SignatureId");

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
                name: "IX_References_ReferenceStatic_ReferenceId",
                table: "References",
                column: "ReferenceStatic_ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_References_ReferenceStaticAttribute_ReferenceId",
                table: "References",
                column: "ReferenceStaticAttribute_ReferenceId");

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
                name: "IX_SimAttributes_InstanceId",
                table: "SimAttributes",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_SimAttributes_ReferenceId",
                table: "SimAttributes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_SimAttributes_RelatedClassId",
                table: "SimAttributes",
                column: "RelatedClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SimClasses_BaseClassId",
                table: "SimClasses",
                column: "BaseClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SimClasses_NamespaceId",
                table: "SimClasses",
                column: "NamespaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SimClassImplements_SimClassId",
                table: "SimClassImplements",
                column: "SimClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SimMethods_RelatedClassId",
                table: "SimMethods",
                column: "RelatedClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SimMethods_ReturnTypeId",
                table: "SimMethods",
                column: "ReturnTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SimNamespaces_BaseNamespaceId",
                table: "SimNamespaces",
                column: "BaseNamespaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "ExecutionLogs");

            migrationBuilder.DropTable(
                name: "ParameterSignatures");

            migrationBuilder.DropTable(
                name: "SimClassImplements");

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

            migrationBuilder.DropTable(
                name: "SimNamespaces");
        }
    }
}
