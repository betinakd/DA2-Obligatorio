using DataAccess;
using DataAccess.Context;
using DataAccess.CustomExceptions;
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

    [TestMethod]
    public void CreateAttribute_ShouldThrowException_WhenDatabaseErrorOccurs()
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

        _context.Dispose();

        Action act = () => _simAttributeDataAccess.CreateAttribute(relatedClassId, attribute);

        act.Should().Throw<DataAccessException>()
            .WithMessage("Data base problem")
            .WithInnerException<DbUpdateException>();
    }

    [TestMethod]
    public void DeleteAttribute_ShouldRemoveAttribute_WhenAttributeExists()
    {
        var relatedClassId = Guid.NewGuid();
        var relatedClass = new SimClass
        {
            Id = relatedClassId,
            Name = "Test Class"
        };
        _simClassDataAccess.CreateSimClass(relatedClass);

        var attributeId = Guid.NewGuid();
        var attribute = new SimAttribute
        {
            Id = attributeId,
            Name = "Test Attribute",
            RelatedClass = relatedClass
        };
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        _simAttributeDataAccess.DeleteAttribute(attributeId);

        var existsAfterDelete = _context.SimAttributes.Any(a => a.Id == attributeId);
        Assert.IsFalse(existsAfterDelete);
    }
}
