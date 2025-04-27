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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configuration(modelBuilder);
        DataSeed(modelBuilder);
    }

    private void Configuration(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SimClass>()
            .HasOne(s => s.BaseClass)
            .WithMany()
            .HasForeignKey(b => b.BaseClassId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SimClass>()
            .HasMany(s => s.Methods)
            .WithOne(m => m.RelatedClass)
            .HasForeignKey(m => m.RelatedClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SimClass>()
            .HasMany(s => s.Attributes)
            .WithOne(m => m.RelatedClass)
            .HasForeignKey(m => m.RelatedClassId);

        modelBuilder.Entity<SimAttribute>()
                    .HasOne(a => a.RelatedClass)
                    .WithMany(c => c.Attributes)
                    .HasForeignKey(a => a.RelatedClassId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SimMethod>()
                    .HasOne(m => m.RelatedClass)
                    .WithMany(c => c.Methods)
                    .HasForeignKey(m => m.RelatedClassId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SimMethod>()
                    .HasMany(m => m.Parameters)
                    .WithOne(p => p.RelatedMethod)
                    .HasForeignKey(p => p.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SimMethod>()
                    .HasMany(m => m.LocalVariables)
                    .WithOne(v => v.RelatedMethod)
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Parameter>()
                    .HasOne(p => p.RelatedMethod)
                    .WithMany(m => m.Parameters)
                    .HasForeignKey(p => p.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Parameter>()
                    .HasOne(p => p.Type)
                    .WithMany()
                    .HasForeignKey(p => p.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LocalVariable>()
                    .HasOne(v => v.RelatedMethod)
                    .WithMany(m => m.LocalVariables)
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LocalVariable>()
                    .HasOne(v => v.Type)
                    .WithMany()
                    .HasForeignKey(v => v.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SimMethod>()
                    .HasMany(i => i.Invocations)
                    .WithOne()
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SimMethod>()
                    .HasOne(m => m.ReturnType)
                    .WithMany()
                    .HasForeignKey(m => m.ReturTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SimAttribute>()
                    .HasOne(a => a.Type)
                    .WithMany()
                    .HasForeignKey(b => b.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invocation>()
                    .HasOne(i => i.RelatedMethod)
                    .WithMany(m => m.Invocations)
                    .HasForeignKey(i => i.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invocation>()
            .HasOne(m => m.Reference)
            .WithOne(r => r.RelatedInvocation)
            .HasForeignKey<Reference>(r => r.RelatedInvocationId);

        modelBuilder.Entity<Invocation>()
            .HasOne(m => m.Signature)
            .WithOne(r => r.RelatedInvocation)
            .HasForeignKey<Signature>(r => r.RelatedInvocationId);

        modelBuilder.Entity<Reference>()
                    .ToTable("References")
                    .HasDiscriminator<string>("ReferenceType")
                    .HasValue<ReferenceThis>("This")
                    .HasValue<ReferenceBase>("Base")
                    .HasValue<ReferenceAttribute>("Attribute")
                    .HasValue<ReferenceParameter>("Parameter")
                    .HasValue<ReferenceVariable>("Variable");

        modelBuilder.Entity<Reference>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Signature>()
            .HasMany<ParameterSignature>()
            .WithOne()
            .HasForeignKey(ps => ps.SignatureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ParameterSignature>()
            .HasOne(ps => ps.Type)
            .WithMany()
            .HasForeignKey(ps => ps.TypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void DataSeed(ModelBuilder modelBuilder)
    {
        var objectClassId = Guid.NewGuid();
        var boolClassId = Guid.NewGuid();

        modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = objectClassId,
                Name = "Object",
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = Guid.NewGuid(),
                Name = "int",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            },
            new SimClass
            {
                Id = Guid.NewGuid(),
                Name = "string",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            },
            new SimClass
            {
                Id = Guid.NewGuid(),
                Name = "float",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            },
            new SimClass
            {
                Id = Guid.NewGuid(),
                Name = "double",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            },
            new SimClass
            {
                Id = Guid.NewGuid(),
                Name = "decimal",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            },
            new SimClass
            {
                Id = Guid.NewGuid(),
                Name = "char",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            },
            new SimClass
            {
                Id = boolClassId,
                Name = "bool",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            });

        modelBuilder.Entity<SimMethod>().HasData(
            new SimMethod
            {
                Id = Guid.NewGuid(),
                Name = "Equals",
                Accesibility = SimAccesibility.Normal,
                Privacity = SimPrivacity.Public,
                RelatedClassId = objectClassId,
                ReturTypeId = boolClassId
            });
    }
}
