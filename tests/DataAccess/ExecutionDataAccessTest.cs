using DataAccess;
using DataAccess.Context;
using Domain;
using Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataAccess;

[TestClass]
public class ExecutionDataAccessTest
{
    private SimulatorDbContext? _context;
    private ExecutionDataAccess? _executionDataAccess;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SimulatorDbContext>()
            .UseInMemoryDatabase(databaseName: "Simulator")
            .Options;

        _context = new SimulatorDbContext(options);
        _executionDataAccess = new ExecutionDataAccess(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void TestFindMethodInHierarchy_ThroughPublicMethods()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var intType = new SimClass { Name = "int" };
        _context.SimClasses.Add(intType);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
            Accesibility = SimAccesibility.Sealed
        };

        var parameter = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType,
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };
        baseMethod.Parameters.Add(parameter);

        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod" };
        var paramSignature = new ParameterSignature
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType,
            SignatureId = signature.Id
        };
        signature.Parameters.Add(paramSignature);

        var result = _executionDataAccess.FindMethodInHierarchy(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
    }

    [TestMethod]
    public void TestFindMethodInHierarchy_ReturnsNull_WhenSimClassIsNull()
    {
        var signature = new Signature { Name = "AnyMethod" };
        var result = _executionDataAccess.FindMethodInHierarchy(null, signature);
        result.Should().BeNull();
    }

    [TestMethod]
    public void TestFindMethodInHierarchy_ReturnsNull_WhenBaseClassNotFound()
    {
        var simClass = new SimClass { Name = "Child", BaseClassId = Guid.NewGuid() };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var signature = new Signature { Name = "AnyMethod" };
        var result = _executionDataAccess.FindMethodInHierarchy(simClass, signature);
        result.Should().BeNull();
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsDirectInheritors()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass1 = new SimClass
        {
            Name = "ChildClass1",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        var childClass2 = new SimClass
        {
            Name = "ChildClass2",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.AddRange(childClass1, childClass2);
        _context.SaveChanges();

        var result = _executionDataAccess.GetAllInheritingClasses(baseClass.Id);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "ChildClass1");
        result.Should().Contain(c => c.Name == "ChildClass2");
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsIndirectInheritors()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var grandChildClass = new SimClass
        {
            Name = "GrandChildClass",
            BaseClassId = childClass.Id,
            BaseClass = childClass
        };
        _context.SimClasses.Add(grandChildClass);
        _context.SaveChanges();

        var result = _executionDataAccess.GetAllInheritingClasses(baseClass.Id);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "ChildClass");
        result.Should().Contain(c => c.Name == "GrandChildClass");
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsEmpty_WhenNoInheritors()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var result = _executionDataAccess.GetAllInheritingClasses(baseClass.Id);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetAllInheritingClasses_ReturnsEmpty_WhenBaseClassDoesNotExist()
    {
        var nonExistentBaseClassId = Guid.NewGuid();

        var result = _executionDataAccess.GetAllInheritingClasses(nonExistentBaseClassId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsFalse_WhenMethodIsNotUsedInInvocation()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = []
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeFalse();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenMethodIsUsedInInvocation()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation],
            Privacity = SimPrivacity.Public
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsFalse_WhenMethodInInheritingIsPrivate()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation]
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeFalse();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenMethodInInheritingIsProtected()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation]
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenMethodInInheritingIsPublic()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation]
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenOwnerClassMethodIsPrivate()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };

        var ownerMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "OwnerMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private,
            Invocations = [invocation]
        };
        baseClass.Methods.Add(ownerMethod);
        _context.SimMethods.Add(ownerMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue();
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsTrue_WhenOwnerClassMethodIsPublic()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethod" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };

        var ownerMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "OwnerMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public,
            Invocations = [invocation]
        };
        baseClass.Methods.Add(ownerMethod);
        _context.SimMethods.Add(ownerMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeTrue("Public methods in owner class should be considered in use by inheriting");
    }

    [TestMethod]
    public void MethodIsInUseByInheriting_ReturnsFalse_WhenMethodInInheritingIsProtectedAndNoMatchesSignature()
    {
        var baseClass = new SimClass { Id = Guid.NewGuid(), Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var baseMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected
        };
        _context.SimMethods.Add(baseMethod);
        _context.SaveChanges();

        var invocation = new Invocation
        {
            Id = Guid.NewGuid(),
            Signature = new Signature { Id = Guid.NewGuid(), Name = "TestMethodnotMatches" },
            RelatedMethodId = baseMethod.Id,
            RelatedMethod = baseMethod
        };

        var childMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "ChildMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation]
        };
        childClass.Methods.Add(childMethod);
        _context.SimMethods.Add(childMethod);
        _context.SaveChanges();

        var result = _executionDataAccess.MethodIsInUseByInheritingInvocations(baseMethod.Id);

        result.Should().BeFalse();
    }

    [TestMethod]
    public void MethodIsOverridingSealed_ReturnsFalse_WhenClassDoesNotExist()
    {
        var nonExistentClassId = Guid.NewGuid();
        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters = []
        };

        var result = _executionDataAccess.MethodIsOverridingSealed(nonExistentClassId, method);

        result.Should().BeFalse("Should return false when class does not exist");
    }

    [TestMethod]
    public void MethodIsOverridingSealed_ReturnsFalse_WhenNoSealedMethodExists()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(simClass);

        var regularMethod = new SimMethod
        {
            Name = "RegularMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Accesibility = SimAccesibility.Normal
        };
        simClass.Methods.Add(regularMethod);
        _context.SimMethods.Add(regularMethod);
        _context.SaveChanges();

        var methodToCheck = new SimMethod
        {
            Name = "RegularMethod",
            Parameters = []
        };

        var result = _executionDataAccess.MethodIsOverridingSealed(simClass.Id, methodToCheck);

        result.Should().BeFalse("Should return false when no sealed method exists with same signature");
    }

    [TestMethod]
    public void MethodIsOverridingSealed_ReturnsTrue_WhenSealedMethodExistsInSameClass()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(simClass);

        var intType = new SimClass { Name = "int" };
        _context.SimClasses.Add(intType);
        _context.SaveChanges();

        var parameter = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType
        };

        var sealedMethod = new SimMethod
        {
            Name = "SealedMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Accesibility = SimAccesibility.Sealed,
            Parameters = [parameter],
            IsVirtual = true,
            IsOverride = true
        };
        parameter.RelatedMethodId = sealedMethod.Id;
        parameter.RelatedMethod = sealedMethod;

        simClass.Methods.Add(sealedMethod);
        _context.SimMethods.Add(sealedMethod);
        _context.SaveChanges();

        var paramToCheck = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType
        };

        var methodToCheck = new SimMethod
        {
            Name = "SealedMethod",
            Parameters = [paramToCheck]
        };

        var result = _executionDataAccess.MethodIsOverridingSealed(simClass.Id, methodToCheck);

        result.Should().BeTrue("Should return true when a sealed method exists with the same signature");
    }

    [TestMethod]
    public void MethodIsOverridingSealed_ReturnsTrue_WhenSealedMethodExistsInBaseClass()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var intType = new SimClass { Name = "int" };
        _context.SimClasses.Add(intType);
        _context.SaveChanges();

        var parameter = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType
        };

        var sealedMethod = new SimMethod
        {
            Name = "SealedMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Accesibility = SimAccesibility.Sealed,
            Parameters = [parameter],
            IsVirtual = true,
            IsOverride = true
        };
        parameter.RelatedMethodId = sealedMethod.Id;
        parameter.RelatedMethod = sealedMethod;

        baseClass.Methods.Add(sealedMethod);
        _context.SimMethods.Add(sealedMethod);
        _context.SaveChanges();

        var paramToCheck = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType
        };

        var methodToCheck = new SimMethod
        {
            Name = "SealedMethod",
            Parameters = [paramToCheck]
        };

        var result = _executionDataAccess.MethodIsOverridingSealed(childClass.Id, methodToCheck);

        result.Should().BeTrue("Should return true when a sealed method exists in base class with the same signature");
    }

    [TestMethod]
    public void MethodIsOverridingSealed_ReturnsTrue_WhenSealedMethodExistsInGrandparentClass()
    {
        var grandparentClass = new SimClass { Name = "GrandparentClass" };
        _context.SimClasses.Add(grandparentClass);
        _context.SaveChanges();

        var parentClass = new SimClass
        {
            Name = "ParentClass",
            BaseClassId = grandparentClass.Id,
            BaseClass = grandparentClass
        };
        _context.SimClasses.Add(parentClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = parentClass.Id,
            BaseClass = parentClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var intType = new SimClass { Name = "int" };
        _context.SimClasses.Add(intType);
        _context.SaveChanges();

        var parameter = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType
        };

        var sealedMethod = new SimMethod
        {
            Name = "SealedMethod",
            RelatedClassId = grandparentClass.Id,
            RelatedClass = grandparentClass,
            Accesibility = SimAccesibility.Sealed,
            Parameters = [parameter],
            IsVirtual = true,
            IsOverride = true
        };
        parameter.RelatedMethodId = sealedMethod.Id;
        parameter.RelatedMethod = sealedMethod;

        grandparentClass.Methods.Add(sealedMethod);
        _context.SimMethods.Add(sealedMethod);
        _context.SaveChanges();

        var paramToCheck = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType
        };

        var methodToCheck = new SimMethod
        {
            Name = "SealedMethod",
            Parameters = [paramToCheck]
        };

        var result = _executionDataAccess.MethodIsOverridingSealed(childClass.Id, methodToCheck);

        result.Should().BeTrue("Should return true when a sealed method exists in grandparent class with the same signature");
    }

    [TestMethod]
    public void MethodIsOverridingSealed_ReturnsFalse_WhenMethodSignatureDiffers()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var intType = new SimClass { Name = "int" };
        var stringType = new SimClass { Name = "string" };
        _context.SimClasses.AddRange(intType, stringType);
        _context.SaveChanges();

        var parameter = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType
        };

        var sealedMethod = new SimMethod
        {
            Name = "SealedMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Accesibility = SimAccesibility.Sealed,
            Parameters = [parameter]
        };
        parameter.RelatedMethodId = sealedMethod.Id;
        parameter.RelatedMethod = sealedMethod;

        baseClass.Methods.Add(sealedMethod);
        _context.SimMethods.Add(sealedMethod);
        _context.SaveChanges();

        var paramToCheck = new Parameter
        {
            Name = "param1",
            TypeId = stringType.Id,
            Type = stringType
        };

        var methodToCheck = new SimMethod
        {
            Name = "SealedMethod",
            Parameters = [paramToCheck]
        };

        var result = _executionDataAccess.MethodIsOverridingSealed(baseClass.Id, methodToCheck);

        result.Should().BeFalse("Should return false when method signature differs (different parameter type)");
    }

    [TestMethod]
    public void SaveExecutionLog_ShouldSaveLogToDatabase()
    {
        var executionLog = new ExecutionLog
        {
            Id = Guid.NewGuid(),
            Execution = "Test execution",
            Reference = "Test reference",
            ObjectCreate = "Test object creation"
        };

        _executionDataAccess!.SaveExecutionLog(executionLog);

        var savedLog = _context!.ExecutionLogs.FirstOrDefault(l => l.Id == executionLog.Id);
        savedLog.Should().NotBeNull("The execution log should be saved to the database");
        savedLog!.Execution.Should().Be("Test execution");
        savedLog.Reference.Should().Be("Test reference");
        savedLog.ObjectCreate.Should().Be("Test object creation");
    }

    [TestMethod]
    public void FindMethodInHierarchy_WithReferenceParameter_LoadsParameterTypesCorrectly()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var intType = new SimClass { Name = "int" };
        _context.SimClasses.Add(intType);
        _context.SaveChanges();

        var methodParameter = new Parameter
        {
            Name = "methodParam",
            TypeId = intType.Id,
            Type = intType,
            Index = 1
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Parameters = [methodParameter]
        };
        methodParameter.RelatedMethod = method;
        methodParameter.RelatedMethodId = method.Id;

        var invocationParameter = new Parameter
        {
            Name = "invocationParam",
            TypeId = intType.Id,
            Type = intType,
            Index = 0
        };

        var referenceParameter = new ReferenceParameter
        {
            Reference = invocationParameter
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            Parameters = []
        };

        var invocation = new Invocation
        {
            Reference = referenceParameter,
            Signature = invocationSignature,
            Index = 0
        };
        method.Invocations.Add(invocation);

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod", Parameters = [new ParameterSignature { Name = "methodParam", TypeId = intType.Id, Type = intType }] };
        var result = _executionDataAccess.FindMethodInHierarchy(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceParameter>();
        var refParam = result.Invocations[0].Reference as ReferenceParameter;
        refParam.Reference.Should().NotBeNull();
        refParam.Reference.Type.Should().NotBeNull();
        refParam.Reference.Type.Name.Should().Be("int");
    }

    [TestMethod]
    public void FindMethodInHierarchy_WithReferenceVariable_LoadsVariableTypesCorrectly()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var stringType = new SimClass { Name = "string" };
        _context.SimClasses.Add(stringType);
        _context.SaveChanges();

        var variable = new LocalVariable
        {
            Name = "testVar",
            TypeId = stringType.Id,
            Type = stringType
        };
        _context.LocalVariables.Add(variable);
        _context.SaveChanges();

        var referenceVariable = new ReferenceVariable
        {
            Reference = variable
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            Parameters = []
        };

        var invocation = new Invocation
        {
            Reference = referenceVariable,
            Signature = invocationSignature,
            Index = 0
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Invocations = [invocation]
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod", Parameters = [] };
        var result = _executionDataAccess.FindMethodInHierarchy(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceVariable>();
        var refVar = result.Invocations[0].Reference as ReferenceVariable;
        refVar.Reference.Should().NotBeNull();
        refVar.Reference.Type.Should().NotBeNull();
        refVar.Reference.Type.Name.Should().Be("string");
    }

    [TestMethod]
    public void FindMethodInHierarchy_WithReferenceAttribute_LoadsAttributeTypesCorrectly()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var boolType = new SimClass { Name = "bool" };
        _context.SimClasses.Add(boolType);
        _context.SaveChanges();

        var attribute = new SimAttribute
        {
            Name = "testAttr",
            TypeId = boolType.Id,
            Type = boolType,
            RelatedClassId = simClass.Id,
            RelatedClass = simClass
        };
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        var referenceAttribute = new ReferenceAttribute
        {
            Reference = attribute
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            Parameters = []
        };

        var invocation = new Invocation
        {
            Reference = referenceAttribute,
            Signature = invocationSignature,
            Index = 0
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Invocations = [invocation]
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod", Parameters = [] };
        var result = _executionDataAccess.FindMethodInHierarchy(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceAttribute>();
        var refAttr = result.Invocations[0].Reference as ReferenceAttribute;
        refAttr.Reference.Should().NotBeNull();
        refAttr.Reference.Type.Should().NotBeNull();
        refAttr.Reference.Type.Name.Should().Be("bool");
    }

    [TestMethod]
    public void FindMethodInHierarchy_WithReferenceBase_LoadsBaseClassCorrectly()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var referenceBase = new ReferenceBase
        {
            Reference = childClass
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            Parameters = []
        };

        var invocation = new Invocation
        {
            Reference = referenceBase,
            Signature = invocationSignature,
            Index = 0
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = childClass.Id,
            RelatedClass = childClass,
            Invocations = [invocation]
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod", Parameters = [] };
        var result = _executionDataAccess.FindMethodInHierarchy(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceBase>();
        var refBase = result.Invocations[0].Reference as ReferenceBase;
        refBase.Reference.Should().NotBeNull();
        refBase.Reference.BaseClass.Should().NotBeNull();
        refBase.Reference.BaseClass.Name.Should().Be("BaseClass");
    }

    [TestMethod]
    public void FindMethodInHierarchy_WithReferenceThis_LoadsThisReferenceCorrectly()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var referenceThis = new ReferenceThis
        {
            Reference = simClass
        };

        var invocationSignature = new Signature
        {
            Name = "InvokedMethod",
            Parameters = []
        };

        var invocation = new Invocation
        {
            Reference = referenceThis,
            Signature = invocationSignature,
            Index = 0
        };

        var method = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Invocations = [invocation]
        };

        _context.SimMethods.Add(method);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod", Parameters = [] };
        var result = _executionDataAccess.FindMethodInHierarchy(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Invocations.Should().HaveCount(1);
        result.Invocations[0].Reference.Should().BeOfType<ReferenceThis>();
        var refThis = result.Invocations[0].Reference as ReferenceThis;
        refThis.Reference.Should().NotBeNull();
        refThis.Reference.Name.Should().Be("TestClass");
    }

    [TestMethod]
    public void IsMethodUsedInClass_WithNullClass_ReturnsFalse()
    {
        var method = new SimMethod { Id = Guid.NewGuid(), Name = "TestMethod" };

        var methodInfo = typeof(ExecutionDataAccess).GetMethod("IsMethodUsedInClass",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (bool)methodInfo.Invoke(_executionDataAccess, [null, method]);

        result.Should().BeFalse("IsMethodUsedInClass should return false for null class");
    }

    [TestMethod]
    public void IsMethodUsedInInheritingClass_WithNullClass_ReturnsFalse()
    {
        var method = new SimMethod { Id = Guid.NewGuid(), Name = "TestMethod" };

        var methodInfo = typeof(ExecutionDataAccess).GetMethod("IsMethodUsedInInheritingClass",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (bool)methodInfo.Invoke(_executionDataAccess, [null, method]);

        result.Should().BeFalse("IsMethodUsedInInheritingClass should return false for null class");
    }

    [TestMethod]
    public void LoadReferenceDetails_WithNullInvocation_DoesNotThrowException()
    {
        var methodInfo = typeof(ExecutionDataAccess).GetMethod("LoadReferenceDetails",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        methodInfo.Invoke(_executionDataAccess, [null]);
    }

    [TestMethod]
    public void OrderMethodsInClass_WithNullClass_DoesNotThrowException()
    {
        var methodInfo = typeof(ExecutionDataAccess).GetMethod("OrderMethodsInClass",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        methodInfo.Invoke(_executionDataAccess, [null]);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_ReturnsNull_WhenSimClassIsNull()
    {
        var signature = new Signature { Name = "AnyMethod" };
        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(null, signature);
        result.Should().BeNull();
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_FindsAnyMethod_InOriginalClass()
    {
        var simClass = new SimClass { Name = "TestClass" };
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();

        var privateMethod = new SimMethod
        {
            Name = "TestMethod",
            RelatedClassId = simClass.Id,
            RelatedClass = simClass,
            Privacity = SimPrivacity.Private
        };
        simClass.Methods.Add(privateMethod);
        _context.SimMethods.Add(privateMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "TestMethod", Parameters = [] };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(simClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("TestMethod");
        result.Privacity.Should().Be(SimPrivacity.Private);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_FindsPublicMethod_InBaseClass()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var publicMethod = new SimMethod
        {
            Name = "PublicMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Public
        };
        baseClass.Methods.Add(publicMethod);
        _context.SimMethods.Add(publicMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "PublicMethod", Parameters = [] };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("PublicMethod");
        result.Privacity.Should().Be(SimPrivacity.Public);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_FindsProtectedMethod_InBaseClass()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var protectedMethod = new SimMethod
        {
            Name = "ProtectedMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Protected
        };
        baseClass.Methods.Add(protectedMethod);
        _context.SimMethods.Add(protectedMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "ProtectedMethod", Parameters = [] };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().NotBeNull();
        result.Name.Should().Be("ProtectedMethod");
        result.Privacity.Should().Be(SimPrivacity.Protected);
    }

    [TestMethod]
    public void FindMethodInHierarchyPublicOrProtected_DoesNotFindPrivateMethod_InBaseClass()
    {
        var baseClass = new SimClass { Name = "BaseClass" };
        _context.SimClasses.Add(baseClass);
        _context.SaveChanges();

        var childClass = new SimClass
        {
            Name = "ChildClass",
            BaseClassId = baseClass.Id,
            BaseClass = baseClass
        };
        _context.SimClasses.Add(childClass);
        _context.SaveChanges();

        var privateMethod = new SimMethod
        {
            Name = "PrivateMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private
        };
        baseClass.Methods.Add(privateMethod);
        _context.SimMethods.Add(privateMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "PrivateMethod", Parameters = [] };

        var result = _executionDataAccess.FindMethodInHierarchyPublicOrProtected(childClass, signature);

        result.Should().BeNull("Private methods in base classes should not be accessible");
    }
}
