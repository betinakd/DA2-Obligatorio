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
            .HasOne(s => s.BaseClass) // una clase tiene UNA clase base
            .WithMany() // una clase base puede tener MUCHAS derivadas (sin propiedad inversa)
            .HasForeignKey("BaseClassId") // FK en la misma tabla
            .OnDelete(DeleteBehavior.Restrict);      // evita eliminar en cascada recursiva

        modelBuilder.Entity<SimAttribute>()
                    .HasOne(a => a.RelatedClass)
                    .WithMany(c => c.Attributes)
                    .HasForeignKey(a => a.RelatedClassId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SimMethod>()
                    .HasOne(m => m.RelatedClass)
                    .WithMany(c => c.Methods)
                    .HasForeignKey(m => m.RelatedClassId)
                    .OnDelete(DeleteBehavior.Cascade);
    }

    private void DataSeed(ModelBuilder modelBuilder)
    {
        var objectClassId = Guid.NewGuid();

        modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = objectClassId,
                Name = "Object",
                State = SimAccesibility.Normal
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
                Id = Guid.NewGuid(),
                Name = "bool",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal
            });
    }
}
