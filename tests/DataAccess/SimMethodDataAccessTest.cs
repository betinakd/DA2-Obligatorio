using DataAccess;
using DataAccess.Context;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class SimMethodDataAccessTest
{
    private SimulatorDbContext? _context;
    private SimMethodDataAccess? _simMethodDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _simMethodDataAccess = new SimMethodDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AddLocalVariable_ShouldAddLocalVariableToDatabase()
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

        var result = _simMethodDataAccess.AddLocalVariable(methodId, localVariable);

        var variableInDb = _context.LocalVariables.FirstOrDefault(v => v.Id == localVariable.Id);
        Assert.IsNotNull(variableInDb);
        Assert.AreEqual(localVariable.Name, variableInDb.Name);
        Assert.AreEqual(localVariable.Type, variableInDb.Type);
        Assert.AreEqual(localVariable.RelatedMethodId, variableInDb.RelatedMethodId);
        Assert.AreEqual(localVariable.Id, result.Id);
    }

    [TestMethod]
    public void AddMethodParameter_ShouldAddParameterToDatabase()
    {
        var methodId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "TestParameter",
            TypeId = typeId,
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "TestType" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };

        var result = _simMethodDataAccess.AddMethodParameter(methodId, parameter);

        var parameterInDb = _context.Parameters.FirstOrDefault(p => p.Id == parameter.Id);
        Assert.IsNotNull(parameterInDb);
        Assert.AreEqual(parameter.Name, parameterInDb.Name);
        Assert.AreEqual(parameter.TypeId, parameterInDb.TypeId);
        Assert.AreEqual(parameter.RelatedMethodId, parameterInDb.RelatedMethodId);
        Assert.AreEqual(parameter.Id, result.Id);
    }

    [TestMethod]
    public void CreateInvocation_ShouldAddInvocationToDatabase()
    {
        var referenceId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            ReferenceId = referenceId,
            MethodName = "Test",
            RelatedMethodId = methodId,
            RelatedMethod = new SimMethod { Id = methodId, Name = "TestRelatedMethod" }
        };

        var result = _simMethodDataAccess.CreateInvocation(methodId, invocation);

        var invocationInDb = _context.Invocations.FirstOrDefault(i => i.Id == invocation.Id);
        Assert.IsNotNull(invocationInDb);
        Assert.AreEqual(invocation.RelatedMethodId, invocationInDb.RelatedMethodId);
        Assert.AreEqual(invocation.Id, result.Id);
        Assert.AreEqual(invocation.ReferenceId, invocationInDb.ReferenceId);
    }
}
