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

        _ = modelBuilder.Entity<Invocation>()
            .Property(i => i.Index)
            .HasDefaultValue(0);

        _ = modelBuilder.Entity<Parameter>()
            .Property(p => p.Index)
            .HasDefaultValue(0);

        _ = modelBuilder.Entity<ParameterSignature>()
            .Property(p => p.Index)
            .HasDefaultValue(0);
    }

    private void DataSeed(ModelBuilder modelBuilder)
    {
        var objectClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var voidTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var boolTypeId = Guid.Parse("22223222-2222-2222-2222-222222222222");

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

        _ = modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = boolTypeId,
                Name = "bool",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            });
    }
}
