using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccess.Context;
public class SimulatorDbContextFactory : IDesignTimeDbContextFactory<SimulatorDbContext>
{
    public SimulatorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SimulatorDbContext>();
        optionsBuilder.UseSqlServer("Server=LAPTOP-VEBF9PUG\\SQLEXPRESS;Database=Simulator;Trusted_Connection=True;TrustServerCertificate=True;User Id=DA2;Password=123");

        return new SimulatorDbContext(optionsBuilder.Options);
    }
}
