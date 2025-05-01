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
        var simAttribute = new SimAttribute()
        {
            Id = Guid.NewGuid(),
            RelatedClassId = simClassId,
            TypeId = typeId,
            Privacity = SimPrivacity.Public,
            Name = "Vehiculo",
            RelatedClass = new SimClass { Id = simClassId, Name = "Test1" },
            Type = new SimClass { Id = typeId, Name = "Test2" }
        };

        _context.SimAttributes.Add(simAttribute);
        _context.SaveChanges();

        var isInUse = _simClassDataAccess!.InUseByOther(typeId);

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

    [TestMethod]
    public void InUseByOther_ShouldReturnTrue_WhenIdIsReferencedInLocalVariables()
    {
        var typeId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var localVariable = new LocalVariable()
        {
            Id = Guid.NewGuid(),
            TypeId = typeId,
            Name = "Test Local Variable",
            RelatedMethodId = methodId,
            Type = new SimClass { Id = typeId, Name = "Test Type" },
            RelatedMethod = new SimMethod { Id = methodId, Name = "Test Related Method" }
        };

        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        var isInUse = _simClassDataAccess!.InUseByOther(typeId);

        Assert.IsTrue(isInUse);
    }

    [TestMethod]
    public void ExistSimClassName_ShouldReturnTrue_WhenNameExists()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "Test Class" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var exists = _simClassDataAccess!.ExistSimClassName("test class");

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldUpdateExistingSimClass()
    {
        var simClassId = Guid.NewGuid();
        var originalSimClass = new SimClass
        {
            Id = simClassId,
            Name = "Original Name",
            BaseClassId = Guid.NewGuid(),
            State = SimAccesibility.Abstract
        };

        _context.SimClasses.Add(originalSimClass);
        _context.SaveChanges();

        var updatedSimClass = new SimClass
        {
            Id = simClassId,
            Name = "Updated Name",
            BaseClassId = Guid.NewGuid(),
            State = SimAccesibility.Normal
        };

        _simClassDataAccess!.UpdateSimClass(updatedSimClass);

        var result = _context.SimClasses.FirstOrDefault(c => c.Id == simClassId);
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Name", result!.Name);
        Assert.AreEqual(updatedSimClass.BaseClassId, result.BaseClassId);
        Assert.AreEqual(SimAccesibility.Normal, result.State);
    }

    [TestMethod]
    public void GetAllSimClasses_ShouldReturnAllSimClasses()
    {
        var simClass1 = new SimClass { Id = Guid.NewGuid(), Name = "Class 1" };
        var simClass2 = new SimClass { Id = Guid.NewGuid(), Name = "Class 2" };

        _context.SimClasses.Add(simClass1);
        _context.SimClasses.Add(simClass2);
        _context.SaveChanges();

        var result = _simClassDataAccess!.GetAllSimClasses();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(c => c.Name == "Class 1"));
        Assert.IsTrue(result.Any(c => c.Name == "Class 2"));
    }

    [TestMethod]
    public void InUseByOther_WithNonMatchingReferenceId_ShouldReturnFalse()
    {
        var id = Guid.NewGuid();
        var reference = new ReferenceThis { Reference = new SimClass { Id = Guid.NewGuid(), Name = "Test Class" } };
        _context.References.Add(reference);
        _context.SaveChanges();

        var result = _simClassDataAccess!.InUseByOther(id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void InUseByOther_WithNullReference_ShouldReturnFalse()
    {
        var id = Guid.NewGuid();
        var reference = new ReferenceThis { Reference = null }; // Simula un caso donde Reference es null
        _context.References.Add(reference);
        _context.SaveChanges();

        var result = _simClassDataAccess!.InUseByOther(id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldRemoveAllMethodRelatedEntities()
    {
        var classId = Guid.NewGuid();
        var methodId = Guid.NewGuid();

        var simClass = new SimClass { Id = classId, Name = "TestClass" };
        _context.SimClasses.Add(simClass);

        var parameter = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            RelatedMethodId = methodId
        };

        var localVariable = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "local1",
            RelatedMethodId = methodId
        };

        var signature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "TestSignature"
        };

        var reference = new ReferenceThis
        {
            Id = Guid.NewGuid(),
            ReferenceId = Guid.NewGuid()
        };

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            RelatedMethodId = methodId,
            Reference = reference,
            Signature = signature
        };

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId,
            RelatedClass = simClass,
            Parameters = [parameter],
            LocalVariables = [localVariable],
            Invocations = [invocation]
        };

        simClass.Methods = [method];

        _context.SimMethods.Add(method);
        _context.Parameters.Add(parameter);
        _context.LocalVariables.Add(localVariable);
        _context.Signatures.Add(signature);
        _context.References.Add(reference);
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        _simClassDataAccess.DeleteSimClass(classId);

        Assert.IsFalse(_context.SimClasses.Any(c => c.Id == classId));
        Assert.IsFalse(_context.SimMethods.Any(m => m.Id == methodId));
        Assert.IsFalse(_context.Parameters.Any(p => p.RelatedMethodId == methodId));
        Assert.IsFalse(_context.LocalVariables.Any(v => v.RelatedMethodId == methodId));
        Assert.IsFalse(_context.Invocations.Any(i => i.RelatedMethodId == methodId));
        Assert.IsFalse(_context.Signatures.Any(s => s.Id == signature.Id));
        Assert.IsFalse(_context.References.Any(r => r.Id == reference.Id));
    }

    [TestMethod]
    public void DeleteSimClass_ShouldHandleMethodWithNullReferenceOrSignature()
    {
        var classId = Guid.NewGuid();
        var methodId = Guid.NewGuid();

        var simClass = new SimClass { Id = classId, Name = "TestClass" };
        _context.SimClasses.Add(simClass);

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            RelatedMethodId = methodId,
            Reference = new ReferenceThis(),
            Signature = new Signature() { Name = "Test" }
        };

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId,
            RelatedClass = simClass,
            Invocations = [invocation]
        };

        simClass.Methods = [method];

        _context.SimMethods.Add(method);
        _context.Invocations.Add(invocation);
        _context.SaveChanges();

        _simClassDataAccess.DeleteSimClass(classId);

        Assert.IsFalse(_context.SimClasses.Any(c => c.Id == classId));
        Assert.IsFalse(_context.SimMethods.Any(m => m.Id == methodId));
        Assert.IsFalse(_context.Invocations.Any(i => i.RelatedMethodId == methodId));
    }

    [TestMethod]
    public void DeleteSimClass_ShouldReturnEarly_WhenSimClassNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        var otherClass = new SimClass { Id = Guid.NewGuid(), Name = "OtherClass" };
        _context.SimClasses.Add(otherClass);
        _context.SaveChanges();

        var initialCount = _context.SimClasses.Count();

        _simClassDataAccess.DeleteSimClass(nonExistentId);

        Assert.AreEqual(initialCount, _context.SimClasses.Count());

        Assert.IsTrue(_context.SimClasses.Any(c => c.Id == otherClass.Id));
    }
}
