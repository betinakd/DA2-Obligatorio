using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context;

public class SimulatorDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SimClass> SimClasses { get; set; }
    public DbSet<SimMethod> SimMethods { get; set; }
    public DbSet<SimAttribute> SimAttributes { get; set; }
    public DbSet<Parameter> Parameters { get; set; }
    public DbSet<LocalVariable> LocalVariables { get; set; }
    public DbSet<Invocation> Invocations { get; set; }

    public DbSet<Reference> References { get; set; }
    public DbSet<Signature> Signatures { get; set; }

    public DbSet<ParameterSignature> ParameterSignatures { get; set; }
    public DbSet<ExecutionLog> ExecutionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configuration(modelBuilder);
        DataSeed(modelBuilder);
    }

    private void Configuration(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.Entity<SimClass>()
            .HasOne(s => s.BaseClass)
            .WithMany()
            .HasForeignKey(b => b.BaseClassId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimClass>()
            .HasMany(s => s.Methods)
            .WithOne(m => m.RelatedClass)
            .HasForeignKey(m => m.RelatedClassId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimClass>()
            .HasMany(s => s.Attributes)
            .WithOne(m => m.RelatedClass)
            .HasForeignKey(m => m.RelatedClassId);

        _ = modelBuilder.Entity<SimAttribute>()
                    .HasOne(a => a.RelatedClass)
                    .WithMany(c => c.Attributes)
                    .HasForeignKey(a => a.RelatedClassId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasOne(m => m.RelatedClass)
                    .WithMany(c => c.Methods)
                    .HasForeignKey(m => m.RelatedClassId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasMany(m => m.Parameters)
                    .WithOne(p => p.RelatedMethod)
                    .HasForeignKey(p => p.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasMany(m => m.LocalVariables)
                    .WithOne(v => v.RelatedMethod)
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Parameter>()
                    .HasOne(p => p.RelatedMethod)
                    .WithMany(m => m.Parameters)
                    .HasForeignKey(p => p.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Parameter>()
                    .HasOne(p => p.Type)
                    .WithMany()
                    .HasForeignKey(p => p.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<LocalVariable>()
                    .HasOne(v => v.RelatedMethod)
                    .WithMany(m => m.LocalVariables)
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<LocalVariable>()
                    .HasOne(v => v.Type)
                    .WithMany()
                    .HasForeignKey(v => v.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasMany(i => i.Invocations)
                    .WithOne()
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasOne(m => m.ReturnType)
                    .WithMany()
                    .HasForeignKey(m => m.ReturnTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimAttribute>()
                    .HasOne(a => a.Type)
                    .WithMany()
                    .HasForeignKey(b => b.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
                    .HasOne(i => i.RelatedMethod)
                    .WithMany(m => m.Invocations)
                    .HasForeignKey(i => i.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
            .HasOne(i => i.Reference)
            .WithOne(r => r.RelatedInvocation)
            .HasForeignKey<Invocation>(i => i.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
            .HasOne(i => i.Signature)
            .WithOne(s => s.RelatedInvocation)
            .HasForeignKey<Invocation>(i => i.SignatureId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
            .HasOne(m => m.Signature)
            .WithOne(r => r.RelatedInvocation)
            .HasForeignKey<Signature>(r => r.RelatedInvocationId);

        _ = modelBuilder.Entity<Reference>()
                    .ToTable("References")
                    .HasDiscriminator<string>("ReferenceType")
                    .HasValue<ReferenceThis>("This")
                    .HasValue<ReferenceBase>("Base")
                    .HasValue<ReferenceAttribute>("Attribute")
                    .HasValue<ReferenceParameter>("Parameter")
                    .HasValue<ReferenceVariable>("Variable");

        _ = modelBuilder.Entity<Reference>()
            .HasKey(r => r.Id);

        _ = modelBuilder.Entity<ReferenceThis>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceAttribute>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceParameter>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceVariable>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceBase>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Signature>()
            .HasMany(s => s.Parameters)
            .WithOne(ps => ps.Signature)
            .HasForeignKey(ps => ps.SignatureId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ParameterSignature>()
            .HasOne(ps => ps.Type)
            .WithMany()
            .HasForeignKey(ps => ps.TypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void DataSeed(ModelBuilder modelBuilder)
    {
        var objectClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var voidTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        _ = modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = objectClassId,
                Name = "Object",
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = voidTypeId,
                Name = "void",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            });

        var motorId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
        var autoId = Guid.Parse("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1");

        var encenderId = Guid.Parse("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1");
        var apagaId = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1");
        var iniciarViajeId = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1");
        var finalizarViajeId = Guid.Parse("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1");

        var motorAutoAttributeId = Guid.Parse("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2");

        var invIniciarViajeEncenderId = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2");
        var invIniciarViajeRecursiveId = Guid.Parse("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2");
        var invFinalizarViajeApagaId = Guid.Parse("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2");

        var firmaEncenderId = Guid.Parse("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2");
        var firmaApagaId = Guid.Parse("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2");
        var firmaIniciarViajeId = Guid.Parse("a3a3a3a3-a3a3-a3a3-a3a3-a3a3a3a3a3a3");

        var motorAutoReferenceId = Guid.Parse("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3");
        var motorAutoReference2Id = Guid.Parse("b8b8b8b8-b8b8-b8b8-b8b8-b8b8b8b8b8b3");
        var thisAutoReferenceId = Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3");
        var thisAutoReference2Id = Guid.Parse("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3");

        _ = modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = motorId,
                Name = "Motor",
                State = SimAccesibility.Normal,
                BaseClassId = objectClassId,
            },
            new SimClass
            {
                Id = autoId,
                Name = "Auto",
                State = SimAccesibility.Normal,
                BaseClassId = objectClassId
            });

        _ = modelBuilder.Entity<SimAttribute>().HasData(
            new SimAttribute
            {
                Id = motorAutoAttributeId,
                Name = "motorAuto",
                TypeId = motorId,
                RelatedClassId = autoId,
                Privacity = SimPrivacity.Private,
            });

        _ = modelBuilder.Entity<SimMethod>().HasData(
            new SimMethod
            {
                Id = encenderId,
                Name = "Encender",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = motorId,
                ReturnTypeId = voidTypeId,
            },
            new SimMethod
            {
                Id = apagaId,
                Name = "Apagar",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = motorId,
                ReturnTypeId = voidTypeId,
            },
            new SimMethod
            {
                Id = iniciarViajeId,
                Name = "IniciarViaje",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = autoId,
                ReturnTypeId = voidTypeId,
            },
            new SimMethod
            {
                Id = finalizarViajeId,
                Name = "FinalizarViaje",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = autoId,
                ReturnTypeId = voidTypeId,
            });

        _ = modelBuilder.Entity<Invocation>().HasData(
            new
            {
                Id = invIniciarViajeEncenderId,
                RelatedMethodId = iniciarViajeId,
                SignatureId = firmaEncenderId,
                ReferenceId = motorAutoReferenceId,
            },
            new
            {
                Id = invIniciarViajeRecursiveId,
                RelatedMethodId = iniciarViajeId,
                SignatureId = firmaIniciarViajeId,
                ReferenceId = thisAutoReferenceId,
            },
            new
            {
                Id = invFinalizarViajeApagaId,
                RelatedMethodId = finalizarViajeId,
                SignatureId = firmaApagaId,
                ReferenceId = motorAutoReference2Id,
            });

        _ = modelBuilder.Entity<Signature>().HasData(
            new
            {
                Id = firmaEncenderId,
                Name = "Encender",
                RelatedInvocationId = invIniciarViajeEncenderId,
            },
            new
            {
                Id = firmaApagaId,
                Name = "Apagar",
                RelatedInvocationId = invFinalizarViajeApagaId,
            },
            new
            {
                Id = firmaIniciarViajeId,
                Name = "IniciarViaje",
                RelatedInvocationId = invIniciarViajeRecursiveId,
            });

        _ = modelBuilder.Entity<ReferenceAttribute>().HasData(
            new
            {
                Id = motorAutoReferenceId,
                ReferenceId = motorAutoAttributeId,
                RelatedInvocationId = invIniciarViajeEncenderId,
                ReferenceType = "Attribute",
            },
            new
            {
                Id = motorAutoReference2Id,
                ReferenceId = motorAutoAttributeId,
                RelatedInvocationId = invIniciarViajeEncenderId,
                ReferenceType = "Attribute",
            });

        _ = modelBuilder.Entity<ReferenceThis>().HasData(
            new
            {
                Id = thisAutoReferenceId,
                ReferenceId = autoId,
                RelatedInvocationId = invIniciarViajeRecursiveId,
                ReferenceType = "This",
            });

        _ = modelBuilder.Entity<ReferenceAttribute>().HasData(
            new
            {
                Id = thisAutoReference2Id,
                ReferenceId = motorAutoAttributeId,
                RelatedInvocationId = invFinalizarViajeApagaId,
                ReferenceType = "Attribute",
            });

        var boolTypeId = Guid.Parse("22223222-2222-2222-2222-222222222222");

        _ = modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = boolTypeId,
                Name = "bool",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            });

        var validadorId = Guid.Parse("a4a2a4a4-a4a4-a4a4-a4a4-a4a4a4a4a4a4");
        var entradaId = Guid.Parse("b4b424b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4");
        var entradaTextoId = Guid.Parse("c4c4c2c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4");
        var entradaTextoEspecialId = Guid.Parse("d4d2d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4");

        var validarId = Guid.Parse("e4e2e4e4-e4e4-e4e4-e4e4-e4e4e4e4e4e4");
        var esValidoEntradaId = Guid.Parse("f4f2f4f4-f4f4-f4f4-f4f4-f4f4f4f4f4f4");
        var esValidoEntradaTextoId = Guid.Parse("a2a5a5a5-a5a5-a5a5-a5a5-a5a5a5a5a5a5");
        var esValidoEntradaTextoEspecialId = Guid.Parse("b2b5b5b5-b5b5-b5b5-b5b5-b5b5b5b5b5b5");
        var limpiarId = Guid.Parse("c5c5c5c5-c5c5-c5c5-c2c5-c5c5c5c5c5c5");
        var inicializarId = Guid.Parse("d5d5d5d5-d5d5-d2d5-d5d5-d5d5d5d5d5d5");

        var inputAttributeId = Guid.Parse("e5e5e2e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5");

        var invValidarInputEsValidoId = Guid.Parse("f2f5f5f5-f5f5-f5f5-f5f5-f5f5f5f5f5f5");
        var invEntradaTextoEspecialBaseEsValidoId = Guid.Parse("a2a6a6a6-a6a6-a6a6-a6a6-a6a6a6a6a6a6");
        var invEntradaTextoBaseEsValidoId = Guid.Parse("b2b6b6b6-b6b6-b6b6-b6b6-b6b6b6b6b6b6");
        var invEntradaTextoThisLimpiarId = Guid.Parse("c2c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6");
        var invEntradaThisInicializarId = Guid.Parse("d2d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6");

        var firmaEsValidoId = Guid.Parse("e6e2e6e6-e6e6-e6e6-e6e6-e6e6e6e6e6e6");
        var firmaLimpiarId = Guid.Parse("f6f2f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6");
        var firmaInicializarId = Guid.Parse("a2a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7");

        var inputReferenceId = Guid.Parse("b7b2b7b7-b7b7-b7b7-b7b7-b7b7b7b7b7b7");
        var thisEntradaReferenceId = Guid.Parse("c2c7c7c7-c7c7-c7c7-c7c7-c7c7c7c7c7c7");
        var thisEntradaTextoReferenceId = Guid.Parse("d2d7d7d7-d7d7-d7d7-d7d7-d7d7d7d7d7d7");
        var baseEntradaTextoReferenceId = Guid.Parse("e2e7e7e7-e7e7-e7e7-e7e7-e7e7e7e7e7e7");
        var baseEntradaTextoEspecialReferenceId = Guid.Parse("f2f7f7f7-f7f7-f7f7-f7f7-f7f7f7f7f7f7");
        var firmaEsValidoBaseEspecialId = Guid.Parse("e6e3e6e6-e6e6-e6e6-e6e6-e6e6e6e6e6e6");
        var firmaEsValidoBaseTextoId = Guid.Parse("f6f3f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6");

        _ = modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = validadorId,
                Name = "Validador",
                State = SimAccesibility.Normal,
                BaseClassId = objectClassId
            },
            new SimClass
            {
                Id = entradaId,
                Name = "Entrada",
                State = SimAccesibility.Normal,
                BaseClassId = objectClassId,
            },
            new SimClass
            {
                Id = entradaTextoId,
                Name = "EntradaTexto",
                BaseClassId = entradaId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = entradaTextoEspecialId,
                Name = "EntradaTextoEspecial",
                BaseClassId = entradaTextoId,
                State = SimAccesibility.Sealed,
            });

        _ = modelBuilder.Entity<SimAttribute>().HasData(
            new SimAttribute
            {
                Id = inputAttributeId,
                Name = "input",
                TypeId = entradaTextoEspecialId,
                RelatedClassId = validadorId,
                Privacity = SimPrivacity.Private,
            });

        _ = modelBuilder.Entity<SimMethod>().HasData(
            new SimMethod
            {
                Id = validarId,
                Name = "Validar",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = validadorId,
                ReturnTypeId = boolTypeId,
            },
            new SimMethod
            {
                Id = esValidoEntradaId,
                Name = "EsValido",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = entradaId,
                ReturnTypeId = boolTypeId,
            },
            new SimMethod
            {
                Id = limpiarId,
                Name = "Limpiar",
                Privacity = SimPrivacity.Protected,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = entradaId,
                ReturnTypeId = voidTypeId,
            },
            new SimMethod
            {
                Id = inicializarId,
                Name = "Inicializar",
                Privacity = SimPrivacity.Protected,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = entradaId,
                ReturnTypeId = voidTypeId,
            },
            new SimMethod
            {
                Id = esValidoEntradaTextoId,
                Name = "EsValido",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = entradaTextoId,
                ReturnTypeId = boolTypeId,
            },
            new SimMethod
            {
                Id = esValidoEntradaTextoEspecialId,
                Name = "EsValido",
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Sealed,
                RelatedClassId = entradaTextoEspecialId,
                ReturnTypeId = boolTypeId,
            });
        _ = modelBuilder.Entity<Invocation>().HasData(
            new
            {
                Id = invValidarInputEsValidoId,
                RelatedMethodId = validarId,
                SignatureId = firmaEsValidoId,
                ReferenceId = inputReferenceId,
            },
            new
            {
                Id = invEntradaTextoEspecialBaseEsValidoId,
                RelatedMethodId = esValidoEntradaTextoEspecialId,
                SignatureId = firmaEsValidoBaseEspecialId,
                ReferenceId = baseEntradaTextoEspecialReferenceId,
            },
            new
            {
                Id = invEntradaTextoBaseEsValidoId,
                RelatedMethodId = esValidoEntradaTextoId,
                SignatureId = firmaEsValidoBaseTextoId,
                ReferenceId = baseEntradaTextoReferenceId,
            },
            new
            {
                Id = invEntradaTextoThisLimpiarId,
                RelatedMethodId = esValidoEntradaTextoId,
                SignatureId = firmaLimpiarId,
                ReferenceId = thisEntradaTextoReferenceId,
            },
            new
            {
                Id = invEntradaThisInicializarId,
                RelatedMethodId = esValidoEntradaId,
                SignatureId = firmaInicializarId,
                ReferenceId = thisEntradaReferenceId,
            });

        _ = modelBuilder.Entity<Signature>().HasData(
            new
            {
                Id = firmaEsValidoId,
                Name = "EsValido",
                RelatedInvocationId = invValidarInputEsValidoId,
            },
            new
            {
                Id = firmaLimpiarId,
                Name = "Limpiar",
                RelatedInvocationId = invEntradaTextoThisLimpiarId,
            },
            new
            {
                Id = firmaInicializarId,
                Name = "Inicializar",
                RelatedInvocationId = invEntradaThisInicializarId,
            },
            new
            {
                Id = firmaEsValidoBaseEspecialId,
                Name = "EsValido",
                RelatedInvocationId = invEntradaTextoEspecialBaseEsValidoId,
            },
            new
            {
                Id = firmaEsValidoBaseTextoId,
                Name = "EsValido",
                RelatedInvocationId = invEntradaTextoBaseEsValidoId,
            });

        _ = modelBuilder.Entity<ReferenceAttribute>().HasData(
            new
            {
                Id = inputReferenceId,
                ReferenceId = inputAttributeId,
                RelatedInvocationId = invValidarInputEsValidoId,
                ReferenceType = "Attribute",
            });

        _ = modelBuilder.Entity<ReferenceThis>().HasData(
            new
            {
                Id = thisEntradaReferenceId,
                ReferenceId = entradaId,
                RelatedInvocationId = invEntradaThisInicializarId,
                ReferenceType = "This",
            },
            new
            {
                Id = thisEntradaTextoReferenceId,
                ReferenceId = entradaTextoId,
                RelatedInvocationId = invEntradaTextoThisLimpiarId,
                ReferenceType = "This",
            });

        _ = modelBuilder.Entity<ReferenceBase>().HasData(
            new
            {
                Id = baseEntradaTextoReferenceId,
                ReferenceId = entradaTextoId,
                RelatedInvocationId = invEntradaTextoBaseEsValidoId,
                ReferenceType = "Base",
            },
            new
            {
                Id = baseEntradaTextoEspecialReferenceId,
                ReferenceId = entradaTextoEspecialId,
                RelatedInvocationId = invEntradaTextoEspecialBaseEsValidoId,
                ReferenceType = "Base",
            });
    }
}
