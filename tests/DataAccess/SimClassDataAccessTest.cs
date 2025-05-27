using DataAccess;
using DataAccess.Context;
using Domain;
using Domain.Enums;
using FluentAssertions;
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
            BaseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
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

    [TestMethod]
    public void GetSimClassById_ShouldOrderCollectionsAndLoadReferences()
    {
        var classId = Guid.NewGuid();
        var methodId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        _context.SimClasses.Add(typeClass);

        var simClass = new SimClass { Id = classId, Name = "TestClass" };
        _context.SimClasses.Add(simClass);

        var param1 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Index = 2,
            RelatedMethodId = methodId,
            TypeId = typeId,
            Type = typeClass
        };
        var param2 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param2",
            Index = 1,
            RelatedMethodId = methodId,
            TypeId = typeId,
            Type = typeClass
        };
        var param3 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param3",
            Index = 0,
            RelatedMethodId = methodId,
            TypeId = typeId,
            Type = typeClass
        };
        _context.Parameters.AddRange(param1, param2, param3);

        var localVar = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "localVar",
            RelatedMethodId = methodId,
            TypeId = typeId,
            Type = typeClass
        };
        _context.LocalVariables.Add(localVar);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "testAttr",
            RelatedClassId = classId,
            TypeId = typeId,
            Type = typeClass
        };
        _context.SimAttributes.Add(attribute);

        var signature = new Signature { Id = Guid.NewGuid(), Name = "TestSignature" };
        var sigParam1 = new ParameterSignature { Id = Guid.NewGuid(), Name = "sigParam1", Index = 2, SignatureId = signature.Id, TypeId = typeId, Type = typeClass };
        var sigParam2 = new ParameterSignature { Id = Guid.NewGuid(), Name = "sigParam2", Index = 0, SignatureId = signature.Id, TypeId = typeId, Type = typeClass };
        signature.Parameters = [sigParam1, sigParam2];
        _context.Signatures.Add(signature);
        _context.ParameterSignatures.AddRange(sigParam1, sigParam2);

        var refParam = new ReferenceParameter { Id = Guid.NewGuid(), ReferenceId = param1.Id, Reference = param1 };
        var refVar = new ReferenceVariable { Id = Guid.NewGuid(), ReferenceId = localVar.Id, Reference = localVar };
        var refAttr = new ReferenceAttribute { Id = Guid.NewGuid(), ReferenceId = attribute.Id, Reference = attribute };
        var refBase = new ReferenceBase { Id = Guid.NewGuid(), ReferenceId = typeId, Reference = typeClass };
        var refThis = new ReferenceThis { Id = Guid.NewGuid(), ReferenceId = classId, Reference = simClass };
        _context.References.AddRange(refParam, refVar, refAttr, refBase, refThis);

        var inv1 = new Invocation { Id = Guid.NewGuid(), Index = 2, RelatedMethodId = methodId, Reference = refParam };
        var inv2 = new Invocation { Id = Guid.NewGuid(), Index = 0, RelatedMethodId = methodId, Reference = refVar };
        var inv3 = new Invocation { Id = Guid.NewGuid(), Index = 4, RelatedMethodId = methodId, Reference = refAttr };
        var inv4 = new Invocation { Id = Guid.NewGuid(), Index = 1, RelatedMethodId = methodId, Reference = refBase };
        var inv5 = new Invocation { Id = Guid.NewGuid(), Index = 3, RelatedMethodId = methodId, Reference = refThis, Signature = signature };
        _context.Invocations.AddRange(inv1, inv2, inv3, inv4, inv5);

        var method = new SimMethod
        {
            Id = methodId,
            Name = "TestMethod",
            RelatedClassId = classId,
            RelatedClass = simClass,
            Parameters = [param1, param2, param3],
            LocalVariables = [localVar],
            Invocations = [inv1, inv2, inv3, inv4, inv5]
        };
        _context.SimMethods.Add(method);

        simClass.Methods = [method];

        _context.SaveChanges();

        var result = _simClassDataAccess.GetSimClassById(classId);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Methods.Count);

        var methodResult = result.Methods[0];

        Assert.AreEqual(3, methodResult.Parameters.Count);
        Assert.AreEqual(0, methodResult.Parameters[0].Index);
        Assert.AreEqual(1, methodResult.Parameters[1].Index);
        Assert.AreEqual(2, methodResult.Parameters[2].Index);

        Assert.AreEqual(5, methodResult.Invocations.Count);
        Assert.AreEqual(0, methodResult.Invocations[0].Index);
        Assert.AreEqual(1, methodResult.Invocations[1].Index);
        Assert.AreEqual(2, methodResult.Invocations[2].Index);
        Assert.AreEqual(3, methodResult.Invocations[3].Index);
        Assert.AreEqual(4, methodResult.Invocations[4].Index);

        var invWithSignature = methodResult.Invocations.FirstOrDefault(i => i.Signature != null);
        Assert.IsNotNull(invWithSignature);
        Assert.IsNotNull(invWithSignature.Signature.Parameters);
    }

    [TestMethod]
    public void GetAllSimClasses_ShouldOrderCollectionsAndLoadReferences()
    {
        var classId1 = Guid.NewGuid();
        var classId2 = Guid.NewGuid();
        var methodId1 = Guid.NewGuid();
        var methodId2 = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        _context.SimClasses.Add(typeClass);

        var simClass1 = new SimClass { Id = classId1, Name = "TestClass1" };
        var simClass2 = new SimClass { Id = classId2, Name = "TestClass2", BaseClassId = classId1, BaseClass = simClass1 };
        _context.SimClasses.AddRange(simClass1, simClass2);

        var param1 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param1",
            Index = 2,
            RelatedMethodId = methodId1,
            TypeId = typeId,
            Type = typeClass
        };
        var param2 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param2",
            Index = 0,
            RelatedMethodId = methodId1,
            TypeId = typeId,
            Type = typeClass
        };
        var param3 = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "param3",
            Index = 1,
            RelatedMethodId = methodId1,
            TypeId = typeId,
            Type = typeClass
        };
        _context.Parameters.AddRange(param1, param2, param3);

        var localVar1 = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "cVar",
            RelatedMethodId = methodId1,
            TypeId = typeId,
            Type = typeClass
        };
        var localVar2 = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "aVar",
            RelatedMethodId = methodId1,
            TypeId = typeId,
            Type = typeClass
        };
        var localVar3 = new LocalVariable
        {
            Id = Guid.NewGuid(),
            Name = "bVar",
            RelatedMethodId = methodId1,
            TypeId = typeId,
            Type = typeClass
        };
        _context.LocalVariables.AddRange(localVar1, localVar2, localVar3);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "testAttr",
            RelatedClassId = classId1,
            TypeId = typeId,
            Type = typeClass
        };
        _context.SimAttributes.Add(attribute);

        var signature = new Signature { Id = Guid.NewGuid(), Name = "TestSignature" };
        var sigParam1 = new ParameterSignature
        {
            Id = Guid.NewGuid(),
            Name = "sigParam1",
            Index = 1,
            SignatureId = signature.Id,
            TypeId = typeId,
            Type = typeClass
        };
        var sigParam2 = new ParameterSignature
        {
            Id = Guid.NewGuid(),
            Name = "sigParam2",
            Index = 0,
            SignatureId = signature.Id,
            TypeId = typeId,
            Type = typeClass
        };
        signature.Parameters = [sigParam1, sigParam2];
        _context.Signatures.Add(signature);
        _context.ParameterSignatures.AddRange(sigParam1, sigParam2);

        var refParam = new ReferenceParameter { Id = Guid.NewGuid(), ReferenceId = param1.Id, Reference = param1 };
        var refVar = new ReferenceVariable { Id = Guid.NewGuid(), ReferenceId = localVar1.Id, Reference = localVar1 };
        var refAttr = new ReferenceAttribute { Id = Guid.NewGuid(), ReferenceId = attribute.Id, Reference = attribute };
        var refBase = new ReferenceBase { Id = Guid.NewGuid(), ReferenceId = classId1, Reference = simClass1 };
        var refThis = new ReferenceThis { Id = Guid.NewGuid(), ReferenceId = classId2, Reference = simClass2 };
        _context.References.AddRange(refParam, refVar, refAttr, refBase, refThis);

        var inv1 = new Invocation { Id = Guid.NewGuid(), Index = 2, RelatedMethodId = methodId1, Reference = refParam };
        var inv2 = new Invocation { Id = Guid.NewGuid(), Index = 0, RelatedMethodId = methodId1, Reference = refVar };
        var inv3 = new Invocation { Id = Guid.NewGuid(), Index = 4, RelatedMethodId = methodId1, Reference = refAttr };
        var inv4 = new Invocation { Id = Guid.NewGuid(), Index = 1, RelatedMethodId = methodId1, Reference = refBase };
        var inv5 = new Invocation { Id = Guid.NewGuid(), Index = 3, RelatedMethodId = methodId1, Reference = refThis, Signature = signature };
        _context.Invocations.AddRange(inv1, inv2, inv3, inv4, inv5);

        var method1 = new SimMethod
        {
            Id = methodId1,
            Name = "ZTestMethod",
            RelatedClassId = classId1,
            RelatedClass = simClass1,
            Parameters = [param1, param2, param3],
            LocalVariables = [localVar1, localVar2, localVar3],
            Invocations = [inv1, inv2, inv3, inv4, inv5]
        };

        var method2 = new SimMethod
        {
            Id = methodId2,
            Name = "ATestMethod",
            RelatedClassId = classId1,
            RelatedClass = simClass1
        };

        _context.SimMethods.AddRange(method1, method2);

        simClass1.Methods = [method1, method2];
        simClass1.Attributes = [attribute];

        _context.SaveChanges();

        var result = _simClassDataAccess.GetAllSimClasses();

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);

        var testClass = result.FirstOrDefault(c => c.Id == classId1);
        Assert.IsNotNull(testClass);

        Assert.AreEqual(2, testClass.Methods.Count);
        Assert.AreEqual("ATestMethod", testClass.Methods[0].Name);
        Assert.AreEqual("ZTestMethod", testClass.Methods[1].Name);

        var methodResult = testClass.Methods[1];

        Assert.AreEqual(3, methodResult.Parameters.Count);
        Assert.AreEqual(0, methodResult.Parameters[0].Index);
        Assert.AreEqual(1, methodResult.Parameters[1].Index);
        Assert.AreEqual(2, methodResult.Parameters[2].Index);

        Assert.AreEqual(3, methodResult.LocalVariables.Count);
        Assert.AreEqual("aVar", methodResult.LocalVariables[0].Name);
        Assert.AreEqual("bVar", methodResult.LocalVariables[1].Name);
        Assert.AreEqual("cVar", methodResult.LocalVariables[2].Name);

        Assert.AreEqual(5, methodResult.Invocations.Count);
        Assert.AreEqual(0, methodResult.Invocations[0].Index);
        Assert.AreEqual(1, methodResult.Invocations[1].Index);
        Assert.AreEqual(2, methodResult.Invocations[2].Index);
        Assert.AreEqual(3, methodResult.Invocations[3].Index);
        Assert.AreEqual(4, methodResult.Invocations[4].Index);

        var invWithSignature = methodResult.Invocations.FirstOrDefault(i => i.Signature != null);
        Assert.IsNotNull(invWithSignature);
        Assert.IsNotNull(invWithSignature.Signature.Parameters);
    }

    [TestMethod]
    public void ClassInheritAttribute_ReturnsFalse_WhenClassDoesNotExist()
    {
        var nonExistentClassId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        var result = _simClassDataAccess.ClassInheritAttribute(nonExistentClassId, attributeId);

        result.Should().BeFalse("Should return false when class does not exist");
    }

    [TestMethod]
    public void ClassInheritAttribute_ReturnsTrue_WhenAttributeFoundInOriginalClass()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        _context.SimClasses.Add(simClass);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "TestAttribute",
            RelatedClassId = simClass.Id,
            Privacity = SimPrivacity.Private
        };
        simClass.Attributes.Add(attribute);
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        var result = _simClassDataAccess.ClassInheritAttribute(simClass.Id, attribute.Id);

        result.Should().BeTrue("Should find attribute in the original class");
    }

    [TestMethod]
    public void ClassInheritAttribute_ReturnsTrue_WhenPublicAttributeFoundInBaseClass()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "TestAttribute",
            RelatedClassId = baseClass.Id,
            Privacity = SimPrivacity.Public
        };
        baseClass.Attributes.Add(attribute);
        _context.SimAttributes.Add(attribute);

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var result = _simClassDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

        result.Should().BeTrue("Should find public attribute in the base class");
    }

    [TestMethod]
    public void ClassInheritAttribute_ReturnsTrue_WhenProtectedAttributeFoundInBaseClass()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "TestAttribute",
            RelatedClassId = baseClass.Id,
            Privacity = SimPrivacity.Protected
        };
        baseClass.Attributes.Add(attribute);
        _context.SimAttributes.Add(attribute);

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var result = _simClassDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

        result.Should().BeTrue("Should find protected attribute in the base class");
    }

    [TestMethod]
    public void ClassInheritAttribute_ReturnsFalse_WhenPrivateAttributeFoundInBaseClass()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "TestAttribute",
            RelatedClassId = baseClass.Id,
            Privacity = SimPrivacity.Private
        };
        baseClass.Attributes.Add(attribute);
        _context.SimAttributes.Add(attribute);

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var result = _simClassDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

        result.Should().BeFalse("Should not find private attribute in the base class");
    }

    [TestMethod]
    public void ClassInheritAttribute_ReturnsFalse_WhenAttributeNotFoundInHierarchy()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var nonExistentAttributeId = Guid.NewGuid();

        var result = _simClassDataAccess.ClassInheritAttribute(childClass.Id, nonExistentAttributeId);

        result.Should().BeFalse("Should return false when attribute not found in hierarchy");
    }

    [TestMethod]
    public void ClassInheritAttribute_ReturnsTrue_WhenAttributeFoundInGrandparentClass()
    {
        var grandparentClass = new SimClass { Id = Guid.NewGuid(), Name = "GrandparentClass" };
        _context.SimClasses.Add(grandparentClass);

        var attribute = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "TestAttribute",
            RelatedClassId = grandparentClass.Id,
            Privacity = SimPrivacity.Public
        };
        grandparentClass.Attributes.Add(attribute);
        _context.SimAttributes.Add(attribute);

        var parentClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ParentClass",
            BaseClassId = grandparentClass.Id,
            BaseClass = grandparentClass
        };
        _context.SimClasses.Add(parentClass);

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = parentClass.Id,
            BaseClass = parentClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var result = _simClassDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

        result.Should().BeTrue("Should find public attribute in the grandparent class");
    }
}
