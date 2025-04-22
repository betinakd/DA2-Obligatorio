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
}
