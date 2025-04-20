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
        DataSeed(modelBuilder);
    }

    private void DataSeed(ModelBuilder modelBuilder)
    {
        var objectClass = new SimClass { Name = "Object", State = SimAccesibility.Normal };
        modelBuilder.Entity<SimClass>().HasData(
            objectClass,
            new SimClass { Name = "int", BaseClass = objectClass },
            new SimClass { Name = "string", BaseClass = objectClass },
            new SimClass { Name = "float", BaseClass = objectClass },
            new SimClass { Name = "double", BaseClass = objectClass },
            new SimClass { Name = "decimal", BaseClass = objectClass },
            new SimClass { Name = "char", BaseClass = objectClass },
            new SimClass { Name = "bool", BaseClass = objectClass });
    }
}
