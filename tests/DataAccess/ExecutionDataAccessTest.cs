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
    public void ClassInheritAttribute_ReturnsFalse_WhenClassDoesNotExist()
    {
        var nonExistentClassId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        var result = _executionDataAccess.ClassInheritAttribute(nonExistentClassId, attributeId);

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
            Privacity = SimPrivacity.Private // Privacidad no importa en nivel 0
        };
        simClass.Attributes.Add(attribute);
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        var result = _executionDataAccess.ClassInheritAttribute(simClass.Id, attribute.Id);

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
            Privacity = SimPrivacity.Public // Atributo público en la clase base
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

        var result = _executionDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

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
            Privacity = SimPrivacity.Protected // Atributo protegido en la clase base
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

        var result = _executionDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

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
            Privacity = SimPrivacity.Private // Atributo privado en la clase base
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

        var result = _executionDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

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

        var result = _executionDataAccess.ClassInheritAttribute(childClass.Id, nonExistentAttributeId);

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

        var result = _executionDataAccess.ClassInheritAttribute(childClass.Id, attribute.Id);

        result.Should().BeTrue("Should find public attribute in the grandparent class");
    }

    [TestMethod]
    public void MethodIsOverridingSealed_ReturnsFalse_WhenClassDoesNotExist()
    {
        // Arrange
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
            Parameters = [parameter]
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
            Parameters = [parameter]
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
}
