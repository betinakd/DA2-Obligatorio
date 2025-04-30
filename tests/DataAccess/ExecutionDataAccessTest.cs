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
    public void TestFindMethodInHierarchy_PrivateMethodsNotInherited()
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

        var privateMethod = new SimMethod
        {
            Name = "PrivateTestMethod",
            RelatedClassId = baseClass.Id,
            RelatedClass = baseClass,
            Privacity = SimPrivacity.Private,
            Accesibility = SimAccesibility.Normal
        };

        var parameter = new Parameter
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType,
            RelatedMethodId = privateMethod.Id,
            RelatedMethod = privateMethod
        };
        privateMethod.Parameters.Add(parameter);

        _context.SimMethods.Add(privateMethod);
        _context.SaveChanges();

        var signature = new Signature { Name = "PrivateTestMethod" };
        var paramSignature = new ParameterSignature
        {
            Name = "param1",
            TypeId = intType.Id,
            Type = intType,
            SignatureId = signature.Id
        };
        signature.Parameters.Add(paramSignature);

        var result = _executionDataAccess.FindMethodInHierarchy(childClass, signature);

        result.Should().BeNull("Private methods should not be found during inheritance lookup");
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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

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

        var result = _executionDataAccess.MethodIsInUseByInheriting(baseMethod.Id);

        result.Should().BeFalse();
    }
}
