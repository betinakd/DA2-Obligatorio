using DataAccess;
using DataAccess.Context;
using Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

public class SimAttributeDataAccessTest
{
    private SimulatorDbContext? _context;
    private SimClassDataAccess? _simClassDataAccess;
    private SimAttributeDataAccess? _simAttributeDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _simClassDataAccess = new SimClassDataAccess(_context);
        _simAttributeDataAccess = new SimAttributeDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void CreateAttribute_ShouldAddAttribute_WhenValidDataProvided()
    {
        var relatedClassId = Guid.NewGuid();
        var relatedClass = new SimClass
        {
            Id = relatedClassId,
            Name = "Test Class"
        };
        _simClassDataAccess.CreateSimClass(relatedClass);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "Test Attribute",
            RelatedClass = relatedClass,
            RelatedClassId = relatedClassId,
            Type = relatedClass,
            TypeId = relatedClassId
        };

        var result = _simAttributeDataAccess.CreateAttribute(relatedClassId, attribute);

        var exists = _context.SimAttributes.Any(a => a.Id == result.Id);
        Assert.IsTrue(exists);
        result.GetType().Should().Be(typeof(SimAttribute));
        Assert.AreEqual(attribute.Name, result.Name);
        Assert.AreEqual(relatedClassId, result.RelatedClassId);
        Assert.AreEqual(relatedClassId, result.TypeId);
    }
}
