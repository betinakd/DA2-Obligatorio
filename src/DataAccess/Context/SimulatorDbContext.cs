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
            .HasForeignKey("BaseClassId")
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
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LocalVariable>()
                    .HasOne(v => v.RelatedMethod)
                    .WithMany(m => m.LocalVariables)
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

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
                    .HasForeignKey("TypeId")
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invocation>()
                    .HasOne(i => i.RelatedMethod)
                    .WithMany(m => m.Invocations)
                    .HasForeignKey(i => i.RelatedMethodId)
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
                Name = "Equals",
                Accesibility = SimAccesibility.Normal,
                RelatedClassId = objectClassId,
                ReturTypeId = boolClassId
            });
    }
}
