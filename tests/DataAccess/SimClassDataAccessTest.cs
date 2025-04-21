using DataAccess;
using DataAccess.Context;
using Domain;
using Domain.Enums;
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

    [TestMethod]
    public void DeleteSimClass_ShouldRemoveSimClassFromDatabase_WhenClassExists()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "Test Class" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        _simClassDataAccess!.DeleteSimClass(simClassId);

        var result = _context.SimClasses.FirstOrDefault(c => c.Id == simClassId);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void InUseByOther_ShouldReturnTrue_WhenIdIsReferencedInBaseClassId()
    {
        var baseClassId = Guid.NewGuid();
        var baseClass = new SimClass { Id = baseClassId, Name = "Base Class" };
        var referencingClass = new SimClass { Id = Guid.NewGuid(), Name = "Referencing Class", BaseClassId = baseClassId, BaseClass = baseClass };

        _context.SimClasses.Add(baseClass);
        _context.SimClasses.Add(referencingClass);
        _context.SaveChanges();

        var isInUse = _simClassDataAccess!.InUseByOther(baseClassId);

        Assert.IsTrue(isInUse);
    }

    [TestMethod]
    public void InUseByOther_ShouldReturnTrue_WhenIdIsReferencedInSimAttributes()
    {
        var simClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var simAttribute = new SimAttribute() { Id = Guid.NewGuid(), RelatedClassId = simClassId, TypeId = typeId, Privacity = SimPrivacity.Public, Name = "Vehiculo", RelatedClass = new SimClass { Id = simClassId, Name = "Test1" }, Type = new SimClass { Id = typeId, Name = "Test2" } };

        _context.SimAttributes.Add(simAttribute);
        _context.SaveChanges();

        var isInUse = _simClassDataAccess!.InUseByOther(simClassId);

        Assert.IsTrue(isInUse);
    }

    [TestMethod]
    public void InUseByOther_ShouldReturnTrue_WhenIdIsReferencedInSimMethods()
    {
        var simClassId = Guid.NewGuid();
        var simMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            RelatedClassId = simClassId,
            Name = "Test Method"
        };

        _context.SimMethods.Add(simMethod);
        _context.SaveChanges();

        var isInUse = _simClassDataAccess!.InUseByOther(simClassId);

        Assert.IsTrue(isInUse);
    }

    [TestMethod]
    public void InUseByOther_ShouldReturnTrue_WhenIdIsReferencedInParameters()
    {
        var typeId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            TypeId = typeId,
            Name = "Test Parameter"
        };

        _context.Parameters.Add(parameter);
        _context.SaveChanges();

        var isInUse = _simClassDataAccess!.InUseByOther(typeId);

        Assert.IsTrue(isInUse);
    }
}
