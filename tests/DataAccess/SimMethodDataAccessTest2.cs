using DataAccess;
using DataAccess.Context;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class SimMethodDataAccessTest2
{
    private SimulatorDbContext? _context;
    private SimMethodDataAccess2? _simMethodDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _simMethodDataAccess = new SimMethodDataAccess2(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void MethodVariableRepeatedValues_ReturnsTrue_WhenVariableExists()
    {
        var typeId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        var localVariableRepeated = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        var result = _simMethodDataAccess.MethodVariableRepeatedValues(methodId, localVariableRepeated);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MethodParameterRepeatedValues_ReturnsTrue_WhenParameterExists()
    {
        var methodId = Guid.NewGuid();
        var parameter = new Parameter { Name = "param1", RelatedMethodId = methodId };
        _context.Parameters.Add(parameter);
        _context.SaveChanges();

        var result = _simMethodDataAccess.MethodParameterRepeatedValues(methodId, new Parameter { Name = "param1" });

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetVariableById_ReturnsVariable_WhenExists()
    {
        var variableId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = variableId,
            TypeId = typeId,
            Name = "TestLocalVariable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };
        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetVariableById(variableId);

        Assert.IsNotNull(result);
        Assert.AreEqual(variableId, result.Id);
        Assert.AreEqual("TestLocalVariable", result.Name);
    }
}
