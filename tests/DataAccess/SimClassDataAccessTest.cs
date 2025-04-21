using DataAccess;
using DataAccess.Context;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class SimClassDataAccessTest
{
    private SimulatorDbContext? _context;
    private SimClassDataAccess? _simClassDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _simClassDataAccess = new SimClassDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void ExistSimClassById_ShouldReturnTrue_WhenClassExists()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "Test Class" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var exists = _simClassDataAccess!.ExistSimClassById(simClassId);

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void GetSimClassById_ShouldReturnSimClass_WhenClassExists()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "Test Class" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var result = _simClassDataAccess!.GetSimClassById(simClassId);

        Assert.IsNotNull(result);
        Assert.AreEqual(simClassId, result.Id);
        Assert.AreEqual("Test Class", result.Name);
    }

    [TestMethod]
    public void CreateSimClass_ShouldAddSimClassToDatabase()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "Test Class" };

        _simClassDataAccess!.CreateSimClass(simClass);

        var result = _context.SimClasses.FirstOrDefault(c => c.Id == simClass.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(simClass.Id, result!.Id);
        Assert.AreEqual(simClass.Name, result.Name);
    }
}
