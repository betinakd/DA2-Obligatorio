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

    [TestMethod]
    public void GetParameterById_ReturnsParameter_WhenExists()
    {
        var parameterId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = parameterId,
            Name = "TestParameter",
            RelatedMethodId = methodId
        };
        _context.Parameters.Add(parameter);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetParameterById(parameterId);

        Assert.IsNotNull(result);
        Assert.AreEqual(parameterId, result.Id);
        Assert.AreEqual("TestParameter", result.Name);
    }

    [TestMethod]
    public void GetMethodById_ReturnsMethod_WhenExists()
    {
        var methodId = Guid.NewGuid();
        var relatedClassId = Guid.NewGuid();
        var simMethod = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = relatedClassId,
            RelatedClass = new SimClass() { Id = relatedClassId, Name = "TestClass" }
        };
        _context.SimMethods.Add(simMethod);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetMethodById(methodId);

        Assert.IsNotNull(result);
        Assert.AreEqual(methodId, result.Id);
        Assert.AreEqual("TestMethod", result.Name);
    }

    [TestMethod]
    public void GetInvocationById_ReturnsInvocation_WhenExists()
    {
        var invocationId = Guid.NewGuid();
        var invocation = new Invocation
        {
            Id = invocationId,
            MethodName = "TestInvocation"
        };
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        var result = _simMethodDataAccess.GetInvocationById(invocationId);

        Assert.IsNotNull(result);
        Assert.AreEqual(invocationId, result.Id);
        Assert.AreEqual("TestInvocation", result.MethodName);
    }

    [TestMethod]
    public void ExistVariableById_ReturnsTrue_WhenVariableExists()
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

        var result = _simMethodDataAccess.ExistVariableById(variableId);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistVariableById_ReturnsFalse_WhenVariableDoesNotExist()
    {
        var variableId = Guid.NewGuid();

        var result = _simMethodDataAccess.ExistVariableById(variableId);

        Assert.IsFalse(result);
    }
}
