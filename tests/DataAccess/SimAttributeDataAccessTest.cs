using DataAccess;
using DataAccess.Context;
using DataAccess.CustomExceptions;
using Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.DataAccess;

public class SimAttributeDataAccessTest
{
    private SimulatorDbContext? _context;
    private SimClassDataAccess? _simClassDataAccess;
    private SimAttributeDataAccess? _simAttributeDataAccess;
    private Mock<SimulatorDbContext>? _mockContext;
    private Mock<DbSet<Invocation>>? _mockInvocations;
    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _simClassDataAccess = new SimClassDataAccess(_context);
        _simAttributeDataAccess = new SimAttributeDataAccess(_context);
        _mockContext = new Mock<SimulatorDbContext>();
        _mockInvocations = new Mock<DbSet<Invocation>>();
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

    [TestMethod]
    public void DeleteAttribute_ShouldThrowException_WhenDatabaseErrorOccurs()
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

        Action act = () => _simAttributeDataAccess.DeleteAttribute(attribute.Id);

        act.Should().Throw<DataAccessException>()
            .WithMessage("Data base problem");
    }

    [TestMethod]
    public void ExistAttributeById_ShouldReturnTrue_WhenAttributeExists()
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

        var exists = _simAttributeDataAccess.ExistAttributeById(attributeId);
        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void ExistAttributeById_ShouldThrowException_WhenDatabaseErrorOccurs()
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
        _context.Dispose();

        Action act = () => _simAttributeDataAccess.ExistAttributeById(attribute.Id);

        act.Should().Throw<DataAccessException>()
            .WithMessage("Data base problem");
    }

    [TestMethod]
    public void ExistAttributeByName_ShouldReturnBool()
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

        var exists = _context.SimAttributes.Any(a => a.Id == attributeId);
        Assert.IsTrue(exists);

        var result = _simAttributeDataAccess.ExistAttributeName(relatedClassId, attribute.Name);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistAttributeByName_ShouldThrowException_WhenDatabaseErrorOccurs()
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

        _context.Dispose();

        Action act = () => _simAttributeDataAccess.ExistAttributeName(relatedClassId,attribute.Name);

        act.Should().Throw<DataAccessException>()
            .WithMessage("Data base problem");
    }

    [TestMethod]
    public void UpdateAttribute_ShouldReturnSimAttribute()
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
            Type = relatedClass,
            TypeId = relatedClassId,
            RelatedClass = relatedClass,
            RelatedClassId = relatedClassId
        };
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        var existingAttribute = _context.SimAttributes.FirstOrDefault(a => a.Id == attributeId);
        Assert.IsNotNull(existingAttribute);

        var newRelatedClassId = Guid.NewGuid();
        var newRelatedClass = new SimClass
        {
            Id = newRelatedClassId,
            Name = "Test Class 2"
        };
        _simClassDataAccess.CreateSimClass(newRelatedClass);

        var newName = "New Name";
        existingAttribute.Name = newName;
        existingAttribute.RelatedClass = newRelatedClass;
        existingAttribute.RelatedClassId = newRelatedClassId;
        existingAttribute.Type = newRelatedClass;
        existingAttribute.TypeId = newRelatedClassId;

        var result = _simAttributeDataAccess.UpdateAttribute(attributeId, existingAttribute);
        result.GetType().Should().Be(typeof(SimAttribute));
        Assert.AreEqual(newName, result.Name);
        Assert.AreEqual(newRelatedClassId, result.RelatedClassId);
    }

    [TestMethod]
    public void UpdateAttribute_ShouldThrowException_WhenDatabaseErrorOccurs()
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
            Type = relatedClass,
            TypeId = relatedClassId,
            RelatedClass = relatedClass,
            RelatedClassId = relatedClassId
        };
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        var existingAttribute = _context.SimAttributes.FirstOrDefault(a => a.Id == attributeId);
        Assert.IsNotNull(existingAttribute);

        var newRelatedClassId = Guid.NewGuid();
        var newRelatedClass = new SimClass
        {
            Id = newRelatedClassId,
            Name = "Test Class 2"
        };
        _context.Dispose();

        Action act = () => _simAttributeDataAccess.UpdateAttribute(attribute.Id,attribute);

        act.Should().Throw<DataAccessException>()
            .WithMessage("Data base problem");
    }

    [TestMethod]
    public void InUseByOther_ShouldReturnTrue_WhenAttributeIsUsed()
    {
        var attributeId = Guid.NewGuid();
        var invocations = new List<Invocation>
        {
            new Invocation
            {
                Id = Guid.NewGuid(),
                Parameters = new List<Parameter>
                {
                    new Parameter { TypeId = attributeId }
                }
            }
        }.AsQueryable();

        _mockInvocations.As<IQueryable<Invocation>>().Setup(m => m.Provider).Returns(invocations.Provider);
        _mockInvocations.As<IQueryable<Invocation>>().Setup(m => m.Expression).Returns(invocations.Expression);
        _mockInvocations.As<IQueryable<Invocation>>().Setup(m => m.ElementType).Returns(invocations.ElementType);
        _mockInvocations.As<IQueryable<Invocation>>().Setup(m => m.GetEnumerator()).Returns(invocations.GetEnumerator());

        _mockContext.Setup(c => c.Invocations).Returns(_mockInvocations.Object);

        var dataAccess = new SimAttributeDataAccess(_mockContext.Object);
        var result = dataAccess.InUseByOther(attributeId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void InUseByOther_ShouldThrowsException_WhenDatabaseErrorOccurs()
    {
        var attributeId = Guid.NewGuid();
        _mockContext.Setup(c => c.Invocations).Throws(new DbUpdateException("Simulated database error"));
        var dataAccess = new SimAttributeDataAccess(_mockContext.Object);

        Action act = () => dataAccess.InUseByOther(attributeId);
        act.Should().Throw<DataAccessException>()
            .WithMessage("Data base problem")
            .WithInnerException<DbUpdateException>();
    }
}
